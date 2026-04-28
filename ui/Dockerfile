FROM node:20-alpine

ENV NPM_CONFIG_UPDATE_NOTIFIER=false

WORKDIR /app

ARG UI_InternalPort
ENV PORT=$UI_InternalPort

COPY package*.json ./

RUN npm install --loglevel=error --no-fund

COPY . .

EXPOSE $PORT

CMD ["npm", "run", "dev", "--", "--host"]