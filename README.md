# Processo Seletivo Take Blip

## 🛠️ Pré-requisitos
- .NET Core 5.0

## 🚀 Execução da aplicação local
Instalação das dependências do projeto:
```bash
dotnet restore
```
Execução do servidor:
```bash
dotnet run
```

## 📄 Sobre a API
### Endpoint
O *endpoint* que se segue tem como resposta a lista dos N repositórios mais antigos de determinado usuário do GitHub, definido pelo parâmetro `username`.
|Verb|URI|Action|
|-----|---|------|
|GET|`<HOST>/api/githubrepositories/{username}`|GitHubRepositoriesController@GetOlderRepositories|
<br/>
### Query Params
Este, por sua vez, permite a utilização de parâmetros para refinamento da busca, são eles:

|Query param|Description|Valor Padrão|Obrigatório|
|---|---|---|---|
|language|Nome da linguagem de programação|null|❌|
|take|Quantidade máxima de repositórios|10|❌|
<br/>
### Exemplo de uso
Obtém os 2 repositórios Java mais antigos da takenet
```bash
GET <HOST>/api/githubrepositories/takenet?language=java&take=2
```
Resposta
```json
[
    {
        "url": "https://api.github.com/repos/takenet/TokenAutoComplete",
        "name": "takenet/TokenAutoComplete",
        "description": "Gmail style MultiAutoCompleteTextView for Android (with the pull request #34 from wdullaer  applied, so we can change the characters to trigger tokenization)",
        "thumbnailUrl": "https://avatars.githubusercontent.com/u/4369522?v=4",
        "language": "Java",
        "createdAt": "2014-11-11T12:45:20"
    },
    {
        "url": "https://api.github.com/repos/takenet/ormlite-android",
        "name": "takenet/ormlite-android",
        "description": "ORMLite Android functionality used in conjunction with ormlite-core",
        "thumbnailUrl": "https://avatars.githubusercontent.com/u/4369522?v=4",
        "language": "Java",
        "createdAt": "2014-12-22T18:01:47"
    }
]
```

## 🦄 Powered by `Heroku`
API publicada no Heroku em:
https://takeblip-chatbot.herokuapp.com/

**Obs:** a aplicação se encontra atrelada a um Free Dyno (unidade de computação gratuita do Heroku), fazendo com que o mesmo durma caso fique 30 minutos sem acessos. Portanto, este pode demorar para dar uma primeira resposta quando algum *endpoint* for chamado.