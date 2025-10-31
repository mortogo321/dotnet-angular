# Multi-stage build for Angular app
FROM node:20-alpine AS build
WORKDIR /app

# Install pnpm
RUN npm install -g pnpm

# Copy package files
COPY package.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile

# Copy source and build
COPY . ./
RUN pnpm run build

# Nginx runtime
FROM nginx:alpine AS final

# Copy built app
COPY --from=build /app/dist/taskboard-app/browser /usr/share/nginx/html

# Copy nginx configuration from docker folder
COPY ../docker/nginx/nginx.conf /etc/nginx/nginx.conf
COPY ../docker/nginx/default.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
