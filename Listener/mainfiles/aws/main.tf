provider "aws" {
  version = "~> 2.0"
  region  = "ap-northeast-1"
}

data "aws_availability_zones" "available" {}

## NETWORKING

# VPC
resource "aws_vpc" "ecs_vpc" {
    //cidr_block = "10.0.0.0/16"
    cidr_block  = "172.17.0.0/16"
}

# Private Subnet
resource "aws_subnet" "ecs_private_subnet" {
    count                   = 2
    cidr_block              = cidrsubnet(aws_vpc.ecs_vpc.cidr_block, 8, count.index)
    availability_zone       = data.aws_availability_zones.available.names[count.index]
    vpc_id                  = aws_vpc.ecs_vpc.id
}


# Public Subnet 1
resource "aws_subnet" "ecs_public_subnet" {
    count                   = 2
    vpc_id                  = aws_vpc.ecs_vpc.id
    cidr_block              = cidrsubnet(aws_vpc.ecs_vpc.cidr_block, 8, 2 + count.index)
    availability_zone       = data.aws_availability_zones.available.names[count.index]
    map_public_ip_on_launch = true
}






# IGW for the public subnet
resource "aws_internet_gateway" "gw" {
    vpc_id                  = aws_vpc.ecs_vpc.id
}

resource "aws_route" "internet_access" {
    route_table_id          = aws_vpc.ecs_vpc.main_route_table_id
    destination_cidr_block  = "0.0.0.0/0"
    gateway_id              = aws_internet_gateway.gw.id
}


resource "aws_eip" "nat_gw" {
    count                     = 2
    vpc                       = true
    depends_on                = [aws_internet_gateway.gw]
}


resource "aws_nat_gateway" "gw" {
    count                       = 2
    subnet_id                   = element(aws_subnet.ecs_public_subnet.*.id, count.index)
    allocation_id               = element(aws_eip.nat_gw.*.id, count.index)       
}


resource "aws_route_table" "private_route" {
    count                         = 2
    vpc_id                  = aws_vpc.ecs_vpc.id
    route {
        cidr_block          = "0.0.0.0/0"
        nat_gateway_id      = element(aws_nat_gateway.gw.*.id, count.index)
    }
}

resource "aws_route_table_association" "private" {
    count                   = 2
    subnet_id               = element(aws_subnet.ecs_private_subnet.*.id, count.index)
    route_table_id          = element(aws_route_table.private_route.*.id, count.index)
}



## SECURITY
# Load Balancer Security Group
resource "aws_security_group" "lb_security_group" {
    name                    = "lb_security_group"
    description             = "ELB Allowed Ports"
    vpc_id                  = aws_vpc.ecs_vpc.id
    ingress {
        cidr_blocks         = ["0.0.0.0/0"]
        protocol            = "tcp"
        from_port           = 8080
        to_port             = 8080
    }
    egress {
        cidr_blocks         = ["0.0.0.0/0"]
        protocol            = "-1"
        from_port           = 0
        to_port             = 0
    }

}


# ECS Security Group
resource "aws_security_group" "ecs_security_group" {
    name                    = "ecs_security_group"
    description             = "ECS Allowed Ports"
    vpc_id                  = aws_vpc.ecs_vpc.id
    ingress {
        cidr_blocks         = ["0.0.0.0/0"]
        protocol            = "tcp"
        from_port           = 8080
        to_port             = 8080
    }
    egress {
        cidr_blocks         = ["0.0.0.0/0"]
        protocol            = "-1"
        from_port           = 0
        to_port             = 0
    }
}


## LOAD BALANCER
resource "aws_alb" "ecs_alb" {
    name                    = "leslb" 
    subnets                 = [ 
                                aws_subnet.ecs_public_subnet.0.id,
                                aws_subnet.ecs_public_subnet.1.id
                              ]
    security_groups         = [aws_security_group.lb_security_group.id]
}


resource "aws_alb_target_group" "ecs_alb_target_group" {
    name                    = "albtargetgroup"
    port                    = 8080
    protocol                = "HTTP"
    vpc_id                  = aws_vpc.ecs_vpc.id
    target_type             = "ip"
}


resource "aws_alb_listener" "front_end" {
    load_balancer_arn       = aws_alb.ecs_alb.id
    port                    = 8080
    protocol                = "HTTP"
    default_action {
        target_group_arn    = aws_alb_target_group.ecs_alb_target_group.id
        type                = "forward"
    }
}

## ECS

# Cluster
resource "aws_ecs_cluster" "ecscluster" {
    name                  = "terraclust"
}

# Task Definition
resource "aws_ecs_task_definition" "ecs_task_def" {
    family                      = "terraform-task-run"
    container_definitions       = file("service.json")
    network_mode                = "awsvpc"
    requires_compatibilities    = ["FARGATE"]
    cpu                         = "256"
    memory                      = "512"
}

resource "aws_ecs_service" "ecs_service" {
    name                        = "terraform_service"
    cluster                     = aws_ecs_cluster.ecscluster.name
    task_definition             = aws_ecs_task_definition.ecs_task_def.family
    desired_count               = 1
    launch_type                 = "FARGATE"

    network_configuration {
        security_groups         = [aws_security_group.ecs_security_group.id]
        subnets                 = [ aws_subnet.ecs_private_subnet.0.id,
                                    aws_subnet.ecs_private_subnet.1.id
                                    
                                    ]
        
    }

    load_balancer {
        target_group_arn        = aws_alb_target_group.ecs_alb_target_group.id
        container_name          = "amazingrace"
        container_port          = 8080
    }

    depends_on                  = [aws_alb_listener.front_end]
}
