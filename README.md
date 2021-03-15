# TweetbookAPI

Code source pour la série de tutorial de Nick Chapsas avec certains ajouts et modification au goût du jour

_https://www.youtube.com/playlist?list=PLUOequmGnXxOgmSDWU7Tl6iQTsOtyjtwU_

## API REST
* API REST de base avec un GET/POST/UPDATE/DELETE en exemple
* Bonnes pratiques pour le versioning des routes
* Utilisation de Swashbuckle pour la documentation OAS3 avec Swagger
* Bonnes pratiques pour l'injection de dépendance avec des "installers"
* Exemple de bonnes pratiques de versioning
* Exemple de bonnes pratiques de découplage entre le domaine et les controllers

### Users
Utilisateurs par défaut créés dans la BD au démarrage:
* poster@gmail.com : Pass1234! (rôle de Poster)
* admin@gmail.com : Pass1234! (rôle d'Admin)
* admin2@nico.com : Pass1234! (rôle d'Admin, nécessaire pour le Delete de tags)
* user@gmail.com : Pass1234! (pas de rôle)

## Sécurité
* Utilisation d'un JWT bearer token
    * Persistence faite dans une BD SQLite avec EF
    * Utilisation de la notion de claims dans les tokens
    * Utilisation de la notion de rôle dans les tokens (la route /tags est accessible pour un rôle Admin seulement)
* Utilisation d'un refresh token
* Utilisation d'une clé d'API pour la route /secrets

## Test de charge
* Image docker avec Artillery.io pour rouler des tests de charge (docker run artillery run test.yaml).

## Test d'intégration
* Tests qui swap la BD SQLite par une BD en mémoire

## Docker
* Dockerfile et docker-compose.yml pour compiler l'application et la rouler dans un container (localhost:5001/swagger)

## Autres
* Utilisation du Option<> de LanguageExtension.Core pour démontrer comment ne plus retourner d'objets null et forcer la validation de la réponse
* Intégration dans le CI de GitHub avec les Action
