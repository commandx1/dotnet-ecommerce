# E-Commerce MVP-2 Design (Dotnet + Postgres + Vue)

## 1. Scope

MVP-2 includes:

- Buyer: register/login, product listing/detail, cart, checkout mock form.
- Vendor: register/login as vendor, own product CRUD, image upload.
- Security: role-based authorization and vendor ownership enforcement.
- Infra: deterministic local startup using `docker compose up -d`.

Out of scope:

- Real payment integration.
- Full order lifecycle and advanced reporting.
- Multi-service decomposition.

## 2. Architecture

Monorepo modular monolith:

- `backend/`
  - `src/Ecommerce.Domain`
  - `src/Ecommerce.Application`
  - `src/Ecommerce.Infrastructure`
  - `src/Ecommerce.Api`
- `frontend/` (Vue 3 + TS + Tailwind + shadcn-vue style component layer)

Why:

- Low operational overhead.
- Strong separation of concerns using Clean Architecture.
- Safe future extraction path if scale requires it.

## 3. Backend Design

### 3.1 Patterns

- Clean Architecture layers.
- CQRS with MediatR.
- FluentValidation for input validation.
- Repository + Unit of Work abstraction.
- ASP.NET Core Identity + JWT access/refresh tokens.

### 3.2 Security

- Roles: `Buyer`, `Vendor`.
- Policy + role-based endpoint protection.
- Vendor-only ownership checks for write operations.
- Refresh token table with rotation metadata.

### 3.3 Data model (PostgreSQL)

Core tables/entities:

- `users` (Identity)
- `products` (belongs to vendor)
- `orders`, `order_items`
- `reviews` (buyer, optional vendor reply)
- `questions` (buyer question, vendor answer)
- `refresh_tokens`

Conventions:

- `timestamptz` semantics via `DateTimeOffset`.
- Audit fields: `CreatedAt`, `UpdatedAt`, optional `IsDeleted`.
- Explicit indexes for foreign keys and key access paths.
- Restrictive delete behaviors to avoid accidental data loss.

## 4. Frontend Design

### 4.1 Stack

- Vue 3 Composition API with `<script setup lang="ts">`.
- TypeScript strict.
- TailwindCSS.
- shadcn-vue style UI layer under `src/components/ui`.
- Pinia stores by domain.
- Vue Router with separated buyer/vendor shells.

### 4.2 Routing

- Buyer routes: `/`, `/products`, `/products/:id`, `/cart`, `/checkout`.
- Vendor routes: `/vendor`, `/vendor/products`.

### 4.3 State

- `auth`: token/session and role state.
- `catalog`: product listing/detail.
- `cart`: local cart and totals.
- `vendorProducts`: vendor-facing product operations.

## 5. Docker & Runtime

Services:

- `postgres`
- `migrator` (runs EF Core migrations and exits)
- `api`
- `frontend`

Boot flow:

1. Postgres healthcheck passes.
2. Migrator applies migrations.
3. API starts.
4. Frontend starts and proxies `/api` to API.

This preserves one-command local startup while keeping migration behavior explicit and auditable.

## 6. Quality Gates

### 6.1 Linters/formatters

- Backend: `dotnet format` + analyzers.
- Frontend: ESLint + Prettier + `vue-tsc`.

### 6.2 Git hooks (Husky)

- `pre-commit`: lint-staged checks.
- `commit-msg`: Conventional Commits (`feat:`, `fix:`, `chore:` etc.).

## 7. Environments

- Local/dev: compose with migrator auto-execution.
- Prod: controlled migration stage in CI/CD using same migrator artifact.

## 8. Risks and Controls

- Risk: role leakage in vendor endpoints.
  - Control: ownership checks in handlers + route-level authorization.
- Risk: startup race conditions.
  - Control: health checks + dependency chain.
- Risk: schema drift.
  - Control: code-first migrations + migrator container.

## 9. Delivery Order

1. Repo bootstrap and ignores.
2. Backend architecture scaffold + auth foundation.
3. Frontend scaffold + routing/store skeleton.
4. Docker compose orchestration with migrator.
5. Lint/Husky/commit conventions.
