# Webshop

This project shows an webshop with a microservice architecture

Technologystack:  
- ASP .NET Core Web API
- Kubernetes
- Istio Proxy
- Envoy
- Prometheus
- Grafana
- gRPC 
- Redis
- Postgres
- RabbitMQ
- Seq

## Application Architecture

Overview of the application architecture:  

![Picture application architecture](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/Architecture.png)

#### Catalog-Service
Handles available products for a web-shop. Provides data via REST and as gRPC-server.  
The services uses an Sqlite-Database to store the products.

|Method|Route|Description|
|---|---|---|
|GET|api/catalog|Returns all product items|
|gRPC|GetItemById|Returns specific product by id|

#### Basket-Service
Handles the basket for customers. Provides data via REST and as gRPC-server.  
The service uses an Redis-Cache for storing the basket items.

When a basket is checked out, an event is created and sent to the rabbitmq message broker.
The Order-service is subscribed to this event and creates an according order.

|Method|Route|Description|
|---|---|---|
|GET|api/basket|Returns all basket items|
|POST|api/basket/checkout|Checks out the customer basket|
|gRPC|GetBasket|Returns all basket items|
|gRPC|UpdateBasket|Updates basket items|

#### Order-Service
Handles the orders for customers. Provides data via REST.  
The service uses an Postgres Database to store orders.

The service is subscribed to BasketCheckout-events in order to create a new order
if a basket is checked out. The orders are then available via the API

|Method|Route|Description|
|---|---|---|
|GET|api/order|Returns a summary of all orders|
|GET|api/order/{id}|Returns a specific order with all details|
|POST|api/order/draft|Creates a new order draft with basket items|

#### Bff-Aggregator
Backend for frontend aggregator - handles use-cases which involves multiple services.  
The aggregator is a gRPC client and communicates to catalog-service and basket-service via gRPC.   

|Method|Route|Description|
|---|---|---|
|POST|api/basket/items|Adds a product to basket|

#### MVC Web-client
ASP .NET Core MVC Web client (Frontend) for the web shop. The webshop is interacting
with the backend through the API-Gaetway.

#### Healthcheck-client
An ASP .NET Core MVC Web client (Frontend) for displaying the Health-Status of all
Services.

![Picture webstatus ui](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/webstatus.PNG)

## Application hosting

Overview of the application hosting architecture:  

![Picture application architecture](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/Architecture_hosting.png)

The application is hosted in a Single-node Kubernetes cluster. (Docker Desktop)

#### Istio 
Istio-Proxy is used as an API Gateway which routes incoming requests to specific kubernetes services.
With enabled Sidecar-Injection, each kubernetes service is deployed within an Istio/Envoy-Proxy. Using this configuration 
a Service Mesh is available (Sidecar's => Data plane and Istio-Proxy => Control plane).

Gatway configuration:

```
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: webshop-gateway
spec:
  selector:
    istio: ingressgateway # use istio default controller
  servers:
  - port:
      number: 80
      name: http
      protocol: HTTP
    hosts:
    - "*"
---
apiVersion: networking.istio.io/v1alpha3
kind: VirtualService
metadata:
  name: webshop
spec:
  hosts:
    - "*"
  gateways:
    - webshop-gateway
  http:
    - match:
      - uri:
          exact: /api/basket/items
      route:
      - destination:
          host: bffaggregator
          port:
            number: 8080
    - match:
      - uri:
          prefix: /api/catalog
      route:
      - destination:
          host: catalog
          port:
            number: 8082
    - match:
      - uri:
          prefix: /api/basket
      route:
      - destination:
          host: basket
          port:
            number: 8084
    - match:
      - uri:
          prefix: /api/order
      route:
      - destination:
          host: order
          port:
            number: 8086
```

#### Prometheus
Prometheus is scrapping Service Metrics from the Istio/Envoy-sidecars and stores them to a database.

Installation:
```
kubectl apply -f https://raw.githubusercontent.com/istio/istio/release-1.10/samples/addons/prometheus.yaml
```

#### Grafana
Grafana comes with an Web-Interface, where the Prometheus database can be used as data source.
The scrapped metrics can be displayed in a dashboard.

Installation:
```
kubectl apply -f https://raw.githubusercontent.com/istio/istio/release-1.10/samples/addons/grafana.yaml
```

Start dashboard: (http://localhost:3000/dashboard/db/istio-mesh-dashboard)
```
istioctl dashboard grafana
```

![Picture grafana](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/grafana.PNG)

#### Seq
For central logging, Seq is used. All the used Services (Web API's) use the Serilog Nuget Package, which is configured, to write logs via HTTP to a central
log server (Seq).

By passing an additional Property with the Service name, the logs can be filtered by each service.

```csharp

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "Order")
    .WriteTo.Console()
    .WriteTo.Seq(seqServerUrl)
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

![Picture Seq](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/seq.PNG)

# Conclusion
With microservice architectures there are some difficulties to deal with. With various tools, these complexities 
can be handled.
With kubernetes we get a mechanism for service discovery, which hosts and keeps up a specified number of service instances.
Also Load balancing is covered by kubernetes.  
With Istio, we get a Service mesh with possibility of monitoring.  
With Seq we can do distributed tracing.  

By using microservice architectures, we can scale applications more efficient and get higher service availability. 

# Screenshots application

![Picture webshop ui](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/shop_1.PNG)
![Picture webshop ui](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/shop_2.PNG)
![Picture webshop ui](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/shop_3.PNG)
![Picture webshop ui](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/shop_4.PNG)
![Picture webshop ui](https://github.com/sve2-2021ss/project-hahn/blob/master/doc/shop_5.PNG)
