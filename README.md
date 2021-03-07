# TweetbookAPI

Code source pour la série de tutorial de Nick Chapsas avec certains ajouts et modification au goût du jour

_https://www.youtube.com/playlist?list=PLUOequmGnXxOgmSDWU7Tl6iQTsOtyjtwU_

## API REST
* API REST de base avec un GET/POST/UPDATE/DELETE en exemple
* Bonnes pratiques pour le versioning des routes
* Setup de Swagger pour avoir un UI toute fait qui documente l'API (avec Swashbuckle)
* Bonnes pratiques pour l'injection de dépendance avec des "installers"

###Users
* poster@gmail.com : Pass1234! (rôle de Poster)
* admin@gmail.com : Pass1234! (rôle d'Admin)
* user@gmail.com : Pass1234! (pas de rôle)

## Sécurité
* Utilisation d'un JWT bearer token
* Persistence faite dans une BD SQLite avec EF
* Utilisation de la notion de claims dans les tokens
* Utilisation de la notion de rôle dans les tokens (la route /tags est accessible pour un rôle Admin seulement)
* Utilisation de refresh token

## Test de charge
* Image docker avec Artillery.io dessus pour rouler des tests de charge (docker run -p 5001:5001 artillery run test.yaml)

## Test d'intégration
* Test qui swap la BD SQLite par une BD en mémoire

## Docker
* Dockerfile et docker-compose.yml pour rouler l'application dans un containers (localhost:7000/swagger)