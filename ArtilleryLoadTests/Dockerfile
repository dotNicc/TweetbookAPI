FROM node:15-alpine

VOLUME /artillery
WORKDIR /artillery

RUN npm install -g npm@7.6.1
RUN npm install -g artillery@1.6

COPY . /artillery

ENTRYPOINT ["artillery"]
CMD ["--help"]