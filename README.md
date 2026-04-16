# dotnet-ecommerce

MVP-2 e-ticaret iskeleti:

- Backend: .NET 8 Web API + Clean Architecture + CQRS + Identity + JWT
- Database: PostgreSQL
- Frontend: Vue 3 + TypeScript + Tailwind + Pinia
- Orchestration: Docker Compose (`postgres -> migrator -> api -> frontend`)

## Tek Komutla Çalıştırma

```bash
docker compose up -d --build
```

Uygulama uçları:

- Frontend: `http://localhost:5173`
- API: compose ağı içinde `http://api:8080` (frontend `/api` proxy ile erişir)

## Seed Kullanıcılar

- Buyer: `buyer@local.dev` / `Passw0rd!`
- Vendor: `vendor@local.dev` / `Passw0rd!`

## Lokal Geliştirme

Frontend:

```bash
cd frontend
npm install
npm run dev
```

Backend:

```bash
dotnet run --project backend/src/Ecommerce.Api
```

## Kod Kalite

- Frontend: ESLint + Prettier + vue-tsc
- Backend: dotnet format + analyzers
- Git hooks: Husky + lint-staged + commitlint (Conventional Commits)
