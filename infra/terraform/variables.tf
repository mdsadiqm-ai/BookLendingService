variable "region" {
  type    = string
  default = "eu-west-2"
}

variable "container_image" {
  type = string
}

variable "container_port" {
  type    = number
  default = 8080
}

variable "desired_count" {
  type    = number
  default = 1
}
