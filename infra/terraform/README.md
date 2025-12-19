# Terraform ECS Fargate deployment

## Prerequisites
1. Terraform 1.5+
2. AWS credentials configured locally
3. A container image URI accessible by ECS, typically in ECR

## Usage
1. terraform init
2. terraform plan -var="container_image=<your image uri>"
3. terraform apply -var="container_image=<your image uri>"

## Output
After apply, Terraform prints the ALB DNS name. You can call:
http://<alb_dns_name>/books
