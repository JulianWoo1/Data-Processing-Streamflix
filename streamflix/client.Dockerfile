# Stage 1: Build
FROM node:20 AS deps
WORKDIR /app
COPY client/client-app/package.json ./
RUN npm cache clean --force && npm install

# Stage 2: Development
FROM node:20
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY --from=deps /app/package-lock.json ./
COPY ./client/client-app ./
EXPOSE 5173
CMD ["npm", "run", "dev"]
