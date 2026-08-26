Installation

Cloner le repository :

git clone https://github.com/romaindhr/Pizzeria.git

Entrer dans le dossier :

cd Pizzeria

Restaurer les dépendances :

dotnet restore

Builder puis lancer l'application :

dotnet build

dotnet run

L'API devrait ensuite être accessible à une adresse similaire à :

https://localhost:7***

ou :

http://localhost:5***

Le port exact est affiché dans le terminal au démarrage.

Swagger

Lorsque l'application est lancée, ouvrir :

https://localhost:7***/swagger

Swagger permet de consulter et tester les endpoints de l'API directement depuis le navigateur.
