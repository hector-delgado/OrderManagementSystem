# Stage 1: Build the Angular application
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Copy package files first for better caching
COPY package*.json ./

# Install dependencies and Angular CLI
RUN npm install
RUN npm install -g @angular/cli@20.1.3

# Copy the rest of the application
COPY . .

# Build the application
RUN ng build --configuration=production

# Stage 2: Serve the application using Nginx
FROM nginx:alpine

# Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*

# Copy the build output to replace the default nginx contents
COPY --from=build /app/dist/OrderManagementUI/browser/* /usr/share/nginx/html/

# Create nginx configuration to handle Angular routing
RUN echo 'server { \
    listen 51688; \
    server_name _; \
    root /usr/share/nginx/html; \
    index index.html; \
    location / { \
        try_files $uri $uri/ /index.html; \
        add_header Cache-Control "no-cache"; \
    } \
    # Deny access to .htaccess files \
    location ~ /\.ht { \
        deny all; \
    } \
    # Enable gzip compression \
    gzip on; \
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript; \
}' > /etc/nginx/conf.d/default.conf

# Expose port 51688
EXPOSE 51688

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
