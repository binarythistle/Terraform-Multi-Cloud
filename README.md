# Multi-cloud with Terraform

This project attempts to run 3 concurrent Terraform processes to provision and deploy the same single container app to each of the 3 major cloud providers: Amazon Web Services, Google Cloud Platform & Microsoft Azure to see which is the fastest. The high-level set up is shown below:

![Solution Overview](/arch.jpg)

This repo comprises 3 Projects:
### Client
(.NET Core 3.1 API). This is what we'll be deploying to the cloud - see **Set Up** below

### Listener
(.NET Core Console app). We'll run 3 separet instances of this app that will listen for a trigger event on the same messgae queue, and upon being "triggered" will kick-off simultanetous provisioning activies (`terraform apply`) on each of the 3 clouds.

The listener project also contains the 3 main.tf files we'll use for each provider, they can be found at:
* [/mainfiles/aws/main.tf](https://github.com/binarythistle/Terraform-Multi-Cloud/blob/master/Listener/mainfiles/aws/main.tf)
* [/mainfiles/azure/main.tf](https://github.com/binarythistle/Terraform-Multi-Cloud/blob/master/Listener/mainfiles/azure/main.tf)
* [/mainfiles/gcp/main.tf](https://github.com/binarythistle/Terraform-Multi-Cloud/blob/master/Listener/mainfiles/gcp/main.tf)

**Note:** The aws `main.tf` file additionally requires a container services json file: [service.json](https://github.com/binarythistle/Terraform-Multi-Cloud/blob/master/Listener/mainfiles/aws/service.json)

### Trigger
(.NET Core Console app). We use this to place a message on the RabbitMQ message bus, (this will trigger the Listeners - see #2).


## Prerequisites
In addition to the projects contained within this repo, there are some other pre-requisites / components you're going to need:

* Terraform [Download & Set Up](https://www.terraform.io/downloads.html)
* Docker Desktop (to run RabbitMQ) [Download & Set Up](https://www.docker.com/products/docker-desktop)
* .NET Core SDK 3.1 [Downlaod & Set Up](https://dotnet.microsoft.com/download)
* Accounts on: AWS, Azure & GCP
* Account on Docker Hub
* *Reccommended*: Non-Interactrive Authentication Set Up for all Cloud Providers
  * [AWS Instructions](https://www.terraform.io/docs/providers/aws/index.html)
  * [Azure Instructions](https://www.terraform.io/docs/providers/azurerm/guides/service_principal_client_secret.html)
  * [GCP Instructions](https://www.terraform.io/docs/providers/google/guides/getting_started.html)

## Set Up

### Run RabbitMQ in Docker
We need a RabbitMQ instance running in Docker, enter the following command to set up:

`docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management`

This will run RabbitMQ on localhost:5672, to test browse to localhost:15672 to view the Management Interface:
* User ID: guest
* Password: guest

### Build & Push Docker Image to Docker Hub
At a command prompt "in" the Client project, build a Docker image of the project:

`docker build -t <Your Docker Hub ID>/<name of image> .`

E.g.:

`docker build -t binarythistle/amazingrace .`

Then push to Docker Hub:

`docker push <Your Docker Hub ID>/<name of image>`

### Push Image to Google Cloud Container Registry

Instructions can be found [here](https://cloud.google.com/container-registry/docs/pushing-and-pulling)

### Update main.tf & service.json files

As you will be using *your* Docker Image, you'll need to update the `main.tf` file, (or in the case of AWS the `service.json` file), to point to your images:

* AWS: Update the `service.json` file to point to your image on **Docker Hub**
* Azure: Update the `main.tf` file to point to your image on **Docker Hub**
* GCP: Update the `main.tf` file to point to your image on **Google Cloud Container Repository**

### Run up 3 Listeners

1. Open 3 separate command prompts, (PowerShell, CMD, Bash etc.)
2. Navigate inside the Listener folder and type: `dotnet run`
3. This should run up the listener, then type the provider you want to use

**NOTE:** Select a different provider for each of the3 3 instances


## Running the Example

**IMPORTANT** The Set up steps above will have to have been performed before you pull the trigger

1. Open another command prompt
2. Navigate inside the "Trigger" folder and type: `dotnet run`
3. Hit any key + `enter` to trigger the 3 listeners 


## Built With

* [.NET Core SDK 3.1](https://dotnet.microsoft.com/download


## Authors

* **Les Jackson**
* **Anthony Dong (Oxalide)** - AWS Fargate `main.tf` file - see Acknowledgements

## Acknowledgments

The Amazon AWS component would not have been possible without the excellent example provided by Anthony Dong:
* [Blog](https://blog.oxalide.io/post/aws-fargate/)
* [GitHub](https://github.com/Oxalide/terraform-fargate-example)
