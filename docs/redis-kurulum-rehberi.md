# Redis + Redis Insight Docker Kurulum Dokümantasyonu

## Amaç

Bu dokümanın amacı:

- Docker üzerinde Redis kurulumu yapmak
- Redis Insight arayüzünü kurmak
- İki container'ı aynı Docker network'ünde çalıştırmak
- Local geliştirme ortamında stabil bir Redis altyapısı oluşturmak
- Olası bağlantı ve cache problemlerini önlemek

---

## Mimari Yapı

Kurulacak yapı:

```
┌─────────────────────┐
│ Redis Insight UI    │
│ localhost:5540      │
└──────────┬──────────┘
           │
           │ Docker Network
           │
┌──────────▼──────────┐
│ Redis Server        │
│ Port: 6379          │
└─────────────────────┘
```

---

## Ön Gereksinimler

Sistemde aşağıdakiler kurulu olmalıdır:

- Docker Desktop
- Docker Engine
- Windows Terminal / PowerShell / CMD

Docker kontrolü:

```bash
docker --version
```

---

## 1. Docker Network Oluşturma

Container'ların birbirleriyle stabil haberleşmesi için özel network oluşturulur.

```bash
docker network create redis-net
```

Kontrol etmek için:

```bash
docker network ls
```

---

## 2. Redis Container Kurulumu

### Temel Redis Kurulumu

```bash
docker run -d ^
--name redis_server ^
--network redis-net ^
-p 6379:6379 ^
redis:latest
```

### Parametre Açıklamaları

| Parametre | Açıklama |
|---|---|
| `-d` | Detached mode (arka planda çalıştırır) |
| `--name` | Container adı |
| `--network` | Dahil olacağı docker network |
| `-p 6379:6379` | Local port eşlemesi |
| `redis:latest` | Kullanılacak image |

---

## 3. Redis Container Kontrolü

Çalışıyor mu kontrol et:

```bash
docker ps
```

Beklenen çıktı:

```
redis_server
```

### Redis Ping Testi

```bash
docker exec -it redis_server redis-cli ping
```

Beklenen çıktı:

```
PONG
```

> Bu çıktı Redis'in çalıştığını doğrular.

---

## 4. Redis Insight Kurulumu

Redis yönetim arayüzü kurulumu:

```bash
docker run -d ^
--name redis_insight ^
--network redis-net ^
-p 5540:5540 ^
redis/redisinsight:latest
```

---

## 5. Redis Insight Arayüzüne Erişim

Browser üzerinden:

```
http://localhost:5540
```

adresine gidilir.

---

## 6. Redis Insight Connection Ayarları

### Add Redis Database

Aşağıdaki alanlar doldurulur:

| Alan | Değer |
|---|---|
| Database Alias | `PharmacyDepot_LocalRedis` |
| Host | `redis_server` |
| Port | `6379` |
| Username | boş |
| Password | boş |

### ⚠️ Kritik Nokta

**✅ DOĞRU**
```
Host: redis_server
```

**❌ YANLIŞ**
```
Host: host.docker.internal
```

**Sebep:**

Container'lar aynı Docker network'ünde olduğundan birbirlerine container adıyla erişmelidir.

Bu yaklaşım:

- Daha stabil
- Daha production-friendly
- Daha sürdürülebilir

bir mimari sağlar.

---

## 7. Redis'e Şifre Ekleme (Opsiyonel)

Şifreli Redis çalıştırmak için:

```bash
docker run -d ^
--name redis_server ^
--network redis-net ^
-p 6379:6379 ^
redis:latest ^
redis-server --requirepass "123456"
```

### Redis Insight Ayarları

| Alan | Değer |
|---|---|
| Password | `123456` |

### Redis CLI Bağlantısı

```bash
docker exec -it redis_server redis-cli
```

Şifre doğrulama:

```
AUTH 123456
```

---

## 8. Docker Volume Kullanımı (Persist Data)

Redis verilerinin container silinse bile kaybolmaması için volume kullanılır.

### Volume Oluşturma

```bash
docker volume create pharmacydepot_redis_data
```

### Volume ile Redis Çalıştırma

```bash
docker run -d ^
--name redis_server ^
--network redis-net ^
-p 6379:6379 ^
-v pharmacydepot_redis_data:/data ^
redis:latest ^
redis-server --appendonly yes
```

---

## 9. Redis Insight Connection Problemleri

### Hata

```
Unsupported encryption strategy
```

### Sebep

Genellikle:

- Browser local storage bozulması
- Redis Insight cache problemi
- Eski container metadata'sı
- Eski encryption strategy cache'i

### Çözüm

**Container Silme**

```bash
docker rm -f redis_insight
```

**Browser Cache Temizleme**

Chrome:

```
chrome://settings/siteData
```

Arama:

```
localhost
```

Sil.

**Local Storage Temizleme**

```
F12 → Application → Local Storage → localhost → Clear
```

---

## 10. Tüm Sistemi Temizleme

### Container'ları Sil

```bash
docker rm -f redis_server
docker rm -f redis_insight
```

### Volume Sil

```bash
docker volume rm pharmacydepot_redis_data
```

### Network Sil

```bash
docker network rm redis-net
```

---

## 11. Faydalı Docker Komutları

### Çalışan Container'lar

```bash
docker ps
```

### Tüm Container'lar

```bash
docker ps -a
```

### Volume Listesi

```bash
docker volume ls
```

### Network Listesi

```bash
docker network ls
```

### Container Logları

```bash
docker logs redis_server
```

### Redis Insight Logları

```bash
docker logs redis_insight
```

---

## 12. Önerilen Production Yaklaşımı

Geliştirme ortamı için:

- Docker Compose
- Persistent Volume
- Şifreli Redis
- Dedicated Network

kullanılması önerilir.

Bir sonraki aşamada:

- Docker Compose
- ASP.NET Core + Redis entegrasyonu
- StackExchange.Redis
- Distributed Cache
- Redis Cache Strategy
- Rate Limiting
- Session Storage

konularına geçilebilir.

---

## 13. Önerilen Klasör Yapısı

```
/docker
    docker-compose.yml

/src
    PharmacyDepot.API
    PharmacyDepot.Application
    PharmacyDepot.Infrastructure
```

---

## 14. İleri Seviye Geliştirme Konuları

İleride aşağıdaki yapılar eklenebilir:

- Redis Pub/Sub
- Distributed Lock
- Redis Sentinel
- Redis Cluster
- Cache Aside Pattern
- CQRS Cache Layer
- Background Job Queue
- SignalR Backplane

---

## 15. Özet

Minimum stabil local geliştirme kurulumu:

1. Docker network oluştur
2. Redis container başlat
3. Redis Insight başlat
4. Container name ile bağlan
5. Volume kullan
6. Browser cache problemlerini temizle
7. Şifreli Redis kullanmayı değerlendir

> Bu yapı modern ASP.NET Core projeleri için yeterli ve sürdürülebilir bir local development altyapısı sağlar.
