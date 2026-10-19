# Multi-stage build for Angular app
# Context: repository root (see docker/docker-compose*.yml)
FROM node:26.12-alpine AS build
WORKDIR /app

# Install pnpm
RUN npm install -g pnpm@11

# Copy package files
COPY frontend/package.json frontend/pnpm-lock.yaml frontend/pnpm-workspace.yaml ./
RUN pnpm install --frozen-lockfile

# Copy source and build
COPY frontend/ ./
RUN pnpm run build

# Nginx runtime
FROM nginx:1.29-alpine AS final

# Copy built app
COPY --from=build /app/dist/taskboard-app/browser /usr/share/nginx/html

# Copy nginx configuration from docker folder
COPY docker/nginx/nginx.conf /etc/nginx/nginx.conf
COPY docker/nginx/default.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:80/health || exit 1

CMD ["nginx", "-g", "daemon off;"]
