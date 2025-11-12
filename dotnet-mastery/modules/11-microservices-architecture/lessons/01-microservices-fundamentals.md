# Lesson 01: Microservices Fundamentals

## 🎯 Learning Objectives

- **Microservices Architecture** - Principles and patterns
- **Service Communication** - REST, gRPC, Message Queues
- **API Gateway** - Centralized entry point
- **Service Discovery** - Finding services dynamically
- **Distributed Tracing** - Tracking requests across services
- **Resilience Patterns** - Circuit Breaker, Retry, Timeout
- **Docker & Kubernetes** - Containerization and orchestration

## 📚 Table of Contents

1. [What are Microservices?](#what-are-microservices)
2. [Monolith vs Microservices](#monolith-vs-microservices)
3. [Communication Patterns](#communication-patterns)
4. [API Gateway](#api-gateway)
5. [Service Discovery](#service-discovery)
6. [Resilience Patterns](#resilience-patterns)
7. [Distributed Tracing](#distributed-tracing)
8. [Docker & Kubernetes](#docker--kubernetes)

---

## What are Microservices?

**Microservices** are small, independent services that work together.

### Characteristics

✅ **Independent**: Each service can be deployed independently
✅ **Focused**: Single business capability per service
✅ **Decentralized**: Each service has its own database
✅ **Resilient**: Failure of one service doesn't affect others
✅ **Technology Agnostic**: Services can use different tech stacks

### Example E-Commerce System

```
┌──────────────────────────────────────────────────┐
│                 API Gateway                       │
└───────────┬──────────────────────────────────────┘
            │
    ┌───────┼───────┬────────┬────────┬────────┐
    │       │       │        │        │        │
┌───▼───┐ ┌─▼──┐ ┌─▼────┐ ┌─▼────┐ ┌─▼────┐ ┌─▼────┐
│Product│ │User│ │Cart  │ │Order │ │Paymt │ │Notif │
│Service│ │Svc │ │Svc   │ │Svc   │ │Svc   │ │Svc   │
└───┬───┘ └─┬──┘ └─┬────┘ └─┬────┘ └─┬────┘ └─┬────┘
    │       │      │        │        │        │
┌───▼───┐ ┌─▼──┐ ┌─▼────┐ ┌─▼────┐ ┌─▼────┐ ┌─▼────┐
│Prod DB│ │User│ │Cart  │ │Order │ │Paymt │ │Queue │
│       │ │DB  │ │DB    │ │DB    │ │DB    │ │      │
└───────┘ └────┘ └──────┘ └──────┘ └──────┘ └──────┘
```

---

## Monolith vs Microservices

### Monolithic Architecture

```csharp
// All features in one application
public class ECommerceApplication
{
    // Product management
    public void AddProduct() { }
    public void UpdateProduct() { }

    // User management
    public void RegisterUser() { }
    public void LoginUser() { }

    // Order processing
    public void CreateOrder() { }
    public void ProcessPayment() { }

    // Everything shares one database
}
```

**Monolith Pros:**
✅ Simple to develop initially
✅ Simple to deploy (one unit)
✅ Simple to test (everything together)

**Monolith Cons:**
❌ Large codebase hard to maintain
❌ Cannot scale components independently
❌ Technology lock-in
❌ Long deployment cycles

### Microservices Architecture

```
Product Service     User Service      Order Service
     ↓                   ↓                  ↓
  Product DB          User DB            Order DB
```

**Microservices Pros:**
✅ Independent deployment
✅ Technology flexibility
✅ Scale services independently
✅ Team autonomy

**Microservices Cons:**
❌ Complex infrastructure
❌ Distributed system challenges
❌ Testing complexity
❌ Data consistency challenges

---

## Communication Patterns

### 1. Synchronous HTTP/REST

```csharp
// Order Service calls Product Service
public class OrderService
{
    private readonly HttpClient _productClient;

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        // Call Product Service to verify stock
        var response = await _productClient.GetAsync(
            $"http://product-service/api/products/{request.ProductId}");

        if (!response.IsSuccessStatusCode)
            throw new Exception("Product not found");

        var product = await response.Content.ReadAsAsync<Product>();

        if (product.Stock < request.Quantity)
            throw new Exception("Insufficient stock");

        // Create order...
        return new Order { /* ... */ };
    }
}
```

### 2. gRPC (High Performance)

```protobuf
// product.proto
syntax = "proto3";

service ProductService {
  rpc GetProduct (GetProductRequest) returns (Product);
  rpc CheckStock (CheckStockRequest) returns (CheckStockResponse);
}

message GetProductRequest {
  int32 product_id = 1;
}

message Product {
  int32 id = 1;
  string name = 2;
  double price = 3;
  int32 stock = 4;
}
```

```csharp
// gRPC Client in Order Service
public class OrderService
{
    private readonly ProductService.ProductServiceClient _productClient;

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var stockResponse = await _productClient.CheckStockAsync(
            new CheckStockRequest
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });

        if (!stockResponse.Available)
            throw new Exception("Insufficient stock");

        // Create order...
    }
}
```

### 3. Asynchronous Messaging (RabbitMQ)

```csharp
// Order Service publishes event
public class OrderService
{
    private readonly IMessageBus _messageBus;

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order { /* ... */ };

        await _repository.SaveAsync(order);

        // Publish event - don't wait for response
        await _messageBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        });

        return order;
    }
}

// Inventory Service subscribes to event
public class InventoryService
{
    [MessageHandler]
    public async Task HandleOrderCreated(OrderCreatedEvent @event)
    {
        // Reduce stock asynchronously
        await _repository.ReduceStockAsync(@event.ProductId, @event.Quantity);
    }
}
```

**Synchronous vs Asynchronous:**

| Aspect | Synchronous (REST/gRPC) | Asynchronous (Messages) |
|--------|-------------------------|-------------------------|
| **Response** | Immediate | Eventual |
| **Coupling** | Tight | Loose |
| **Failure** | Cascade | Isolated |
| **Use When** | Need immediate answer | Fire-and-forget |

---

## API Gateway

**API Gateway** is the single entry point for all client requests.

### Without API Gateway (Bad)

```
Mobile App ────┐
               ├──→ Product Service
Web App ───────┤
               ├──→ User Service
Desktop App ───┤
               ├──→ Order Service
Partner API ───┘
               └──→ Payment Service

❌ Clients must know all service URLs
❌ Cross-cutting concerns duplicated (auth, logging)
❌ Complex client code
```

### With API Gateway (Good)

```
Mobile App ────┐
               │
Web App ───────┼──→ API Gateway ──┬──→ Product Service
               │                   ├──→ User Service
Desktop App ───┤                   ├──→ Order Service
               │                   └──→ Payment Service
Partner API ───┘

✅ Single entry point
✅ Authentication, rate limiting in one place
✅ Request routing and composition
```

### Implementing API Gateway (Ocelot)

```bash
dotnet add package Ocelot
```

```json
// ocelot.json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/products/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "product-service",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/products/{id}",
      "UpstreamHttpMethod": [ "GET" ]
    },
    {
      "DownstreamPathTemplate": "/api/orders",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "order-service",
          "Port": 80
        }
      ],
      "UpstreamPathTemplate": "/orders",
      "UpstreamHttpMethod": [ "GET", "POST" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer"
      },
      "RateLimitOptions": {
        "EnableRateLimiting": true,
        "Period": "1m",
        "Limit": 100
      }
    }
  ]
}
```

---

## Service Discovery

**Service Discovery** allows services to find each other dynamically.

### Consul Service Discovery

```csharp
// Register service on startup
public class Startup
{
    public void Configure(IApplicationBuilder app, IConsulClient consul)
    {
        var registration = new AgentServiceRegistration
        {
            ID = "product-service-1",
            Name = "product-service",
            Address = "10.0.0.5",
            Port = 5000,
            Check = new AgentServiceCheck
            {
                HTTP = "http://10.0.0.5:5000/health",
                Interval = TimeSpan.FromSeconds(10)
            }
        };

        consul.Agent.ServiceRegister(registration).Wait();
    }
}

// Discover service when needed
public class OrderService
{
    private readonly IConsulClient _consul;

    public async Task<Product> GetProductAsync(int productId)
    {
        // Query Consul for healthy product service instances
        var services = await _consul.Health.Service("product-service", null, true);
        var service = services.Response.First();

        var url = $"http://{service.Service.Address}:{service.Service.Port}/api/products/{productId}";

        // Make HTTP request...
    }
}
```

---

## Resilience Patterns

### 1. Circuit Breaker (Polly)

```bash
dotnet add package Polly
```

```csharp
// Prevent cascading failures
public class ProductService
{
    private readonly IAsyncPolicy<HttpResponseMessage> _circuitBreaker;

    public ProductService()
    {
        _circuitBreaker = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,  // Open after 3 failures
                durationOfBreak: TimeSpan.FromSeconds(30)  // Stay open for 30s
            );
    }

    public async Task<Product> GetProductAsync(int id)
    {
        var response = await _circuitBreaker.ExecuteAsync(() =>
            _httpClient.GetAsync($"/api/products/{id}"));

        return await response.Content.ReadAsAsync<Product>();
    }
}
```

**Circuit Breaker States:**
```
Closed (Normal) ──[3 failures]──→ Open (Reject requests)
      ↑                                    │
      │                                    │ [30s timeout]
      │                                    ↓
      └──[Success]──── Half-Open (Try one request)
```

### 2. Retry Pattern

```csharp
// Automatically retry failed requests
var retryPolicy = Policy
    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .Or<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),  // 2s, 4s, 8s
        onRetry: (outcome, timespan, retryCount, context) =>
        {
            _logger.LogWarning(
                "Retry {RetryCount} after {Delay}s",
                retryCount, timespan.TotalSeconds);
        });

var response = await retryPolicy.ExecuteAsync(() =>
    _httpClient.GetAsync(url));
```

### 3. Timeout Pattern

```csharp
// Prevent hanging requests
var timeoutPolicy = Policy
    .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));

var response = await timeoutPolicy.ExecuteAsync(() =>
    _httpClient.GetAsync(url));
```

### 4. Combining Policies

```csharp
// Circuit Breaker + Retry + Timeout
var policy = Policy.WrapAsync(
    _circuitBreaker,
    _retryPolicy,
    _timeoutPolicy
);

var response = await policy.ExecuteAsync(() =>
    _httpClient.GetAsync(url));
```

---

## Distributed Tracing

**Distributed Tracing** tracks requests across multiple services.

### OpenTelemetry

```bash
dotnet add package OpenTelemetry.Exporter.Jaeger
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
```

```csharp
// Configure tracing
builder.Services.AddOpenTelemetryTracing(builder =>
{
    builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("order-service"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter(options =>
        {
            options.AgentHost = "jaeger";
            options.AgentPort = 6831;
        });
});

// Traces automatically created for:
// - Incoming HTTP requests
// - Outgoing HTTP requests
// - Database calls (with EF Core instrumentation)
```

**Trace Example:**
```
Request ID: abc123

API Gateway     [────────────────────] 250ms
  ├─ Order Svc  [──────────] 150ms
  │   ├─ DB     [──] 20ms
  │   └─ Product [────] 80ms
  │       └─ DB  [──] 15ms
  └─ Payment Svc [────] 70ms
      └─ External API [───] 50ms
```

---

## Docker & Kubernetes

### Dockerfile for .NET Service

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["ProductService/ProductService.csproj", "ProductService/"]
RUN dotnet restore "ProductService/ProductService.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ProductService"
RUN dotnet build "ProductService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.dll"]
```

### Docker Compose (Local Development)

```yaml
# docker-compose.yml
version: '3.8'

services:
  api-gateway:
    image: api-gateway:latest
    build:
      context: .
      dockerfile: ApiGateway/Dockerfile
    ports:
      - "5000:80"
    depends_on:
      - product-service
      - order-service

  product-service:
    image: product-service:latest
    build:
      context: .
      dockerfile: ProductService/Dockerfile
    environment:
      - ConnectionStrings__DefaultConnection=Server=product-db;Database=Products
    depends_on:
      - product-db

  product-db:
    image: postgres:15
    environment:
      - POSTGRES_PASSWORD=password
    volumes:
      - product-data:/var/lib/postgresql/data

  order-service:
    image: order-service:latest
    build:
      context: .
      dockerfile: OrderService/Dockerfile
    environment:
      - ConnectionStrings__DefaultConnection=Server=order-db;Database=Orders
      - RabbitMQ__Host=rabbitmq
    depends_on:
      - order-db
      - rabbitmq

  order-db:
    image: postgres:15
    environment:
      - POSTGRES_PASSWORD=password
    volumes:
      - order-data:/var/lib/postgresql/data

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "15672:15672"  # Management UI
    environment:
      - RABBITMQ_DEFAULT_USER=admin
      - RABBITMQ_DEFAULT_PASS=password

volumes:
  product-data:
  order-data:
```

### Kubernetes Deployment

```yaml
# product-service-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: product-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: product-service
  template:
    metadata:
      labels:
        app: product-service
    spec:
      containers:
      - name: product-service
        image: myregistry/product-service:latest
        ports:
        - containerPort: 80
        env:
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: db-secrets
              key: connection-string
        resources:
          requests:
            memory: "128Mi"
            cpu: "250m"
          limits:
            memory: "256Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /ready
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5

---
apiVersion: v1
kind: Service
metadata:
  name: product-service
spec:
  selector:
    app: product-service
  ports:
  - port: 80
    targetPort: 80
  type: ClusterIP

---
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: product-service-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: product-service
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

---

## Summary

**Key Takeaways:**

1. **Microservices**: Small, independent, focused services
2. **Communication**: REST (simple), gRPC (fast), Messages (async)
3. **API Gateway**: Single entry point for all clients
4. **Service Discovery**: Services find each other dynamically
5. **Resilience**: Circuit Breaker, Retry, Timeout prevent cascading failures
6. **Tracing**: OpenTelemetry tracks requests across services
7. **Docker**: Containerize services for consistency
8. **Kubernetes**: Orchestrate containers at scale

**When to Use Microservices:**
✅ Large, complex applications
✅ Multiple teams
✅ Independent deployment requirements
✅ Different scalability needs per feature

**When NOT to Use:**
❌ Small applications
❌ Tight deadlines
❌ Small team
❌ Simple CRUD apps

---

**Ready to build?** Start with a monolith and split into microservices when needed!
