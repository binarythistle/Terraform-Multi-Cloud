provider "azurerm"{
    version = "2.5.0"
    features {}
}

resource "azurerm_resource_group" "tf_amazingrace"{
    name = "amazingrace"
    location = "japaneast"
}

resource "azurerm_container_group" "tfcg_amazingrace"{
    name                        = "amazingracebill"
    location                    = azurerm_resource_group.tf_amazingrace.location
    resource_group_name         = azurerm_resource_group.tf_amazingrace.name

    ip_address_type             = "public"
    dns_name_label              = "amazingracebill"
    os_type                     = "linux"

    container {
       name                     = "amazingracebill"
       image                    = "binarythistle/raceclient8080"
       cpu                      = "1"
       memory                   = "1"
       ports {
           port                 = 8080
           protocol             = "TCP"
       }
       environment_variables = { ClientName = "AZURE"}
    }

}