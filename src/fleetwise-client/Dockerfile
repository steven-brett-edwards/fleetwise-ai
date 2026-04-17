# syntax=docker/dockerfile:1

# ---- build stage ----------------------------------------------------------
FROM node:22-alpine AS build
WORKDIR /app

# Install deps first so they're cached independently of source changes.
COPY package.json package-lock.json ./
RUN npm ci --no-audit --no-fund

COPY . .
RUN npm run build -- --configuration production

# ---- runtime stage --------------------------------------------------------
FROM nginx:alpine AS runtime

# Angular 20+ with the application builder emits to dist/<project>/browser/.
COPY --from=build /app/dist/fleetwise-client/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

# nginx image's default CMD is fine.
