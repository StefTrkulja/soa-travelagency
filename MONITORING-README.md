# Travel Agency - Logging & Monitoring

Implementirane su **2 od 3** zahtevane komponente: **LOGGING** i **MONITORING**

## 🎯 Implementirane Komponente

### 1. **LOGGING** 📝
- **Serilog** u .NET servisima (stakeholders, tour, gateway)
- **Spring Boot** logging u Java servisu (blog)
- **Elasticsearch** za čuvanje i agregaciju logova
- **Kibana** za vizualizaciju i pretragu logova

### 2. **MONITORING** 📊
- **Prometheus** za skupljanje metrika
- **Grafana** za dashboards i vizualizaciju
- **Node Exporter** - metrike host mašine (CPU, RAM, disk, network)
- **cAdvisor** - metrike kontejnera (CPU, RAM, disk, network)

## 🚀 Pokretanje

```bash
# Pokreni Docker Desktop prvo!
cd "d:\SOA Projekat"
docker-compose up --build
```

## 🌐 Pristup UI-jima

| Servis | URL | Kredencijali | Svrha |
|--------|-----|--------------|--------|
| **API Gateway** | http://localhost:5000 | - | Glavni API |
| **Swagger** | http://localhost:5000/swagger | - | API dokumentacija |
| **Kibana (Logovi)** | http://localhost:5601 | - | Pretraga logova |
| **Grafana (Monitoring)** | http://localhost:3000 | admin/admin | Dashboards |
| **Prometheus** | http://localhost:9090 | - | Metrike |
| **cAdvisor** | http://localhost:8080 | - | Container stats |

## 📊 Metrike - Ispunjeni Zahtevi

### **Host Metrike** (Node Exporter):
✅ **CPU usage** - procesor iskorišćenost  
✅ **RAM usage** - memorija iskorišćenost  
✅ **Filesystem** - disk prostor  
✅ **Network traffic** - mrežni saobraćaj  

### **Container Metrike** (cAdvisor):
✅ **CPU per container** - CPU po kontejneru  
✅ **RAM per container** - memorija po kontejneru  
✅ **Filesystem per container** - disk I/O po kontejneru  
✅ **Network per container** - network I/O po kontejneru  

## 📝 Logging - Ispunjeni Zahtevi

✅ **Agregacija logova** implementirana  
✅ **Kibana vizualizacija** dostupna  
✅ **Strukturirani logovi** u JSON formatu  
✅ **Različiti servisi** - stakeholders, tour, gateway, blog  

### Kibana Setup:
1. http://localhost:5601
2. Create index pattern: `*service*` 
3. Filter po servisu: `ServiceName: "stakeholders-service"`

## 🧪 Testiranje

### Test API pozivi (generiše logove):
```bash
# Registracija
curl -X POST http://localhost:5000/api/gateway/stakeholders/users/register \
  -H "Content-Type: application/json" \
  -d '{
    "email":"test@example.com",
    "password":"Password123!",
    "firstName":"Test",
    "lastName":"User"
  }'

# Login
curl -X POST http://localhost:5000/api/gateway/stakeholders/users/login \
  -H "Content-Type: application/json" \
  -d '{
    "email":"test@example.com",
    "password":"Password123!"
  }'
```

### Uloge u sistemu (iz zahteva):
- **Administrator** - upravljanje sistemom
- **Vodič (kreator tura)** - kreiranje i upravljanje turama  
- **Turista** - pretraživanje i rezervacija tura

## 📁 Gde se čuvaju logovi:
- **Lokalno**: `./logs/` direktorijum
- **Elasticsearch**: za pretragu i analizu
- **Console**: real-time praćenje

## 🎛️ Grafana Dashboards:
- Host CPU & Memory usage
- Container resource usage
- Network & disk I/O
- Application metrics

## ✅ Zahtevi ispunjeni:
- ✅ Logging agregacija sa vizualizacijom
- ✅ Host metrike (CPU, RAM, disk, network)  
- ✅ Container metrike (CPU, RAM, disk, network)
- ✅ 2 od 3 komponente implementirane (3 poena)