# Análise dos requisitos:
## Requisitos funcionais:
- RF1: Registrar lançamentos (créditos e débitos) no fluxo de caixa.
- RF2: Atualizar lançamentos existentes.
- RF3: Excluir lançamentos.
- RF4: Consultar lançamentos.
- RF5: Gerar relatório diário consolidado do fluxo de caixa.
- RF6: Sincronização entre os serviços de controle de lançamentos e consolidação diária.

## Requisitos não funcionais:
- RNF1: O serviço de consolidado diário recebe 50 requisições por segundo, com no máximo 5% de perda de requisições.
- RNF2: O serviço de controle de lançamento não deve ficar indisponível se o sistema de consolidado diário cair.
- RNF3: Implementar estratégias de recuperação de falhas.
- RNF4: Implementação de autenticação, autorização e criptografia.
- RNF5: Garantir alta disponibilidade e escalabilidade para ambos os serviços.
- RNF6: Otimização de desempenho, disponibilidade e confiabilidade.
- RNF7: Implementação de testes unitários.
- RNF8: Implementação de monitoramento e observabilidade.
- RNF9: Deve ser feito usando C#

# Analise de domínios.
- Domínio 1: Lançamentos (transactions)
  - Registrar lançamentos.
  - Atualizar lançamentos.
  - Excluir lançamentos.
  - Consultar lançamentos.
- Domínio 2: Consolidação Diária (consolidation)
  - Processar o saldo diário consolidado.
 
# Diagrama de contexto
![ACT - Carrefour](https://github.com/user-attachments/assets/bb139388-6770-4afa-8ca8-874708994e29)

## Detalhamento da arquitetura
- Arquitetura segue um padrão de microsserviços orquestrados em um ambiente de contêineres (Docker) gerenciados por Kubernetes ou Swarm. A estrutura é composta por três microsserviços principais:
  - MS Auth.
  - MS Transaction.
  - MS Consolidation.
- A comunicação entre os microsserviços é mediada por um Load Balancer NGINX, que gerencia as requisições HTTP. O MySQL atua como banco de dados persistente, enquanto o Redis funciona como cache, com fallback para o banco de dados em caso de falha no cache. Atualmente os dois microsserviços compartilham o mesmo banco de dados, evitando assim inconsistência de dados. Caso necessário, para uma evolução, os bancos podem ser divididos e as informações compartilhadas por mensageria ou ETL, podendo haver inconsistência parcial de dados, porém tornando os microsserviços desacoplados no que diz respeito ao banco de dados.
### Frontend (Não implementado):
- Função: Interface de usuário para consumo dos serviços via HTTP.
- Detalhamento: Frontend com código estático podendo ser acoplado junto ao servidor ou disponibilizado com serviço de CDN como cloudfront da AWS com distribuição geográfica muito performática. Não implementado para este desafio.

### Microsserviços:
#### MS Auth (Authentication Service) (Implementado parcialmente)
- Função: Emite e valida tokens JWT. Implementado parcialmente, o suficiente para promover autenticação para os MS.
- Endpoints:
  - `POST /Auth/login` - Gera um token JWT.
  - `POST /Auth/validate` - Valida um token JWT.
#### MS Transaction (Transaction Service)
- Função: CRUD das transações financeiras.
- Endpoints:
  - `GET /Transaction/{id}` - Busca por ID.
  - `GET /Transaction` - Busca paginada.
  - `POST /Transaction` - Criação de transação.
  - `PUT /Transaction/{id}` - Atualização de transação.
  - `DELETE /Transaction/{id}` - Exclusão de transação.
- Mensageria:
  - TransactionPublisher: Publica mensagens na fila RabbiMQ para comunicação com ms consolidation para invalidação do cache.
#### MS Consolidation (Consolidation Service)
- Função: Consolidação diária de dados financeiros.
- Endpoints:
    - `GET /Consolidation` - Consulta de consolidação por data.
- Mensageria:
  - RabbitMQConsumer: Listener que fica ouvindo a fila alimentada pelo ms transaction, processando mensagens e invalidando cache.
### Infraestrutura:
#### Nginx (Load Balancer)
- Função: Balanceamento de carga entre instâncias dos microsserviços.
- Motivo: Simplicidade de configuração e suporte a múltiplas instâncias de serviços.
#### MySQL (Database)
- Função: Banco de dados relacional para persistência de transações.
- Motivo: Simplicidade de implementação e suporte a operações transacionais.
#### Redis (Cache)
- Função: Armazenamento em memória para cache de consolidações.
- Motivo: Reduzir a carga no banco de dados e melhorar a velocidade de consulta.
#### RabbitMQ (Message Broker)
- Função: Mensageria entre ms transaction e ms consolidation para notificações de mudanças.
- Motivo: Garantir comunicação assíncrona e desacoplamento entre microsserviços.
#### Prometheus (Monitoramento e Coleta de Métricas)
- Função: Monitorar métricas do sistema, incluindo requisições HTTP, latências e status dos serviços.
- Motivo: Implementar observabilidade, permitindo a análise em tempo real do comportamento dos microsserviços, identificação de gargalos e possíveis falhas.

# Estruturas, componentes e detalhamento dos serviços.
## Estrutura e componetes dos MS's.
- Arquitetura modular seguindo os princípios de Clean Architecture
- Divisão em camadas
  - Api: Controladores e DTOs (interface pública).
  - Application: Serviços, interfaces e validações.
  - Domain: Entidades, enums e interfaces de repositório (lógica de negócios).
  - Infrastructure: Implementações de repositórios, mensagens e contexto de banco.
  - Migrations: Estrutura para versionamento do banco de dados.
  - Tests: Estrutura de testes unitários, cobrindo serviços e repositórios.
- Princípios SOLID aplicados em toda a aplicação
  - SRP (Single Responsibility Principle): Cada camada possui um único propósito (API, Serviço, Repositório).
  - DIP (Dependency Inversion Principle): Serviços dependem de interfaces, não de implementações concretas.
- Estrutura e componentes
  - Api:
    - Controllers:
      - TransactionController.cs: Exposição dos endpoints GET, POST, PUT, DELETE.
    - DTOs:
      - Parameters: Definições de parâmetros de consulta.
      - Requests: Estrutura dos requests (CreateTransactionRequest, UpdateTransactionRequest).
      - Responses: Estrutura dos responses (TransactionResponse).
  - Application:
    - Services: Implementações dos serviços de transações (TransactionService).
    - Interfaces: Interfaces de serviços (ITransactionService, IMessageService).
    - Exceptions: Exceções customizadas (BusinessException, NotFoundException).
    - Mappers: Mapeamento entre DTOs, entidades e modelos de banco.
  - Domain:
    - Entities: Entidades principais (TransactionEntity).
    - Enums: Enumerações para tipagem (TransactionType).
    - Interfaces: Interfaces de repositório (ITransactionRepository).
  - Infrastructure:
    - Data: Contexto do banco de dados (AppDbContext).
    - EFModels: Modelos específicos do Entity Framework (TransactionModel).
    - Repositories: Implementações de repositórios (TransactionRepository).
    - Messaging: Implementação do serviço de mensageria (RabbitMQPublisher).
  - Migrations:
    - Scripts de migração gerados automaticamente com base no modelo TransactionModel.
  - Tests:
    - UnitTests: Testes de unidade para serviços (TransactionServiceTests).
- Fluxo das informações
  - Requisição HTTP recebida pelo Controller.
  - Dados mapeados para entidades no Mapper.
  - Validações e exceções tratadas no Service.
  - Caso necessário, acessa cache ou passa a consumir banco por fallback.
  - Operações no banco de dados realizadas pelo Repository.
  - Caso necessário, mensagem publicada no RabbitMQ para atualização do cache.
  - Resposta gerada e enviada para o cliente pelo Controller.
- Validações e exceções
  - Validações realizadas nos DTOs de requests e classes de domínio. (por conta do tempo do desafio nem todas as validações foram cobertas).
  - Para exceções deve ser usado classe personalizada que centraliza a captura e devolve exceções com status code correto de acordo com o cenário, as principais devem ser:
    -  BusinessException (400): Falhas de negócio (ex.: campo obrigatório).
    -  NotFoundException (404): Transação não encontrada.
    -  UnauthorizedException (401): Falha na autorização.
- Mensageria
  - Formato da mensagem: JSON contendo a data da transação e o ID da transação.
  - Motivação: Notificar o serviço MS Consolidation para invalidar o cache após operações de transação.
- Cache
  - Implementação no service, para algumas rotas de consulta, consultando primeiro no cache e depois no banco por fallback (Por conta do tempo do desafio, apenas consolidation tem implementaçãod e cache.)
- Banco de dados
  - Banco relacional configurado com Entity Framework Core.
  - Estrutura da tabela transactions:
    - Id (GUID) - Chave primária.
    - Date (DateTime) - Data da transação.
    - Value (Decimal) - Valor da transação.
    - Description (String) - Descrição.
    - CreatedAt (DateTime) - Data de criação.
    - UpdatedAt (DateTime) - Data da última atualização.
- Testes
  - Centralização dos testes realizados no microsserviço, para este desafio: unitários e performance.
  - Implementação e cobertura.
    - Testes unitários com cobertura apenas na camada de serviço.
    - Para este desafio, apenas consolidation tem teste de performance com K6.
- Logs e observabilidade
  - Logs estruturados utilizando ILogger<TransactionService>. (Por questão do tempo do desafio, apenas a camada de serviço contempla logs estruturados).
  - Estrutura dos logs:
    - LogInformation() - Operações de sucesso.
    - LogWarning() - Exceções de validação e transação não encontrada.
    - LogError() - Erros críticos durante operações.
- Segurança.
  - Todas as rotas são protegidas por JWT. (Por conta do tempo do desafio a proteção de rotas foi aplicada apenas em transaction)
  - Autenticação realizada pelo MS Auth.

# Instruções para rodar a aplicação e testes.
## Pré-requisitos
- Docker e Docker Compose instalados.
  - Portas livres:
    - 80 (Nginx)
    - 3300 (MySQL)
    - 6379 (Redis)
    - 5672 (RabbitMQ)
    - 15672 (RabbitMQ Management)
    - 9000 (Prometheus)
- .NET SDK 8.0 instalado. 
- EF CLI (Entity Framework Core CLI): Instalar globalmente: `dotnet tool install --global dotnet-ef` (Para rodar migrations e testes unitários)
- K6 para testes de performance (Atualmente apenas em consolidation)
  - (Windows) Baixar e instalar: [https://k6.io/open-source](https://k6.io/open-source/)/
  - (Windows) Via Choco: `choco install k6`
  - (MacOS) Via brew: `brew install k6`
  - (Linux) Via apt: `sudo apt install k6`
- Estrutura do Projeto
  - docker-compose.yml - Definição dos serviços.
  - act-ms-transaction - Microsserviço de transações.
  - act-ms-consolidation - Microsserviço de consolidação.
  - act-ms-auth - Microsserviço de autenticação.
  - nginx.conf - Configuração do balanceamento de carga.
## Rodar a aplicação
- Passo 1: Construir as Imagens
  - No diretório raiz do projeto, execute: `docker-compose build`
- Passo 2: Subir os Containers
  - `docker-compose up -d`
- Passo 3: Aplicação das Migrations
  - No diretorio raiz do projeto act-ms-transaction, execute: `dotnet ef database update`
- Rotas do swagger para efetuar as requisições:
  - http://localhost/auth/swagger/index.html
  - http://localhost/transaction/swagger/index.html
  - http://localhost/consolidation/swagger/index.html
## Executar os testes
- Unitários:
  - Na raiz da solução, execute: `dotnet test`
- Performance:
  - Na pasta Tests/PerformanceTests do ms consolidation, execute: `k6 run ConsolidationPerformanceTest.js`.
# Validação dos requisitos funcionais e não funcionais:
## Pré-requisitos:
- Docker funcional com todos os containers em pleno funcionamento.
- Migrations do ms transactions aplicadas.
- Pegar o token de acesso:
  - Acesse o ms auth para pegar token JWT: `http://localhost/auth/swagger/index.html`
  - No swagger, acesse a rota POST Auth/login > Try it out.
  - Envie o payload de acesso (Já está configurado o email e senha que está hardcode para gerar o token)  
`{
  "username": "admin",
  "password": "123456"
}`
  - Exemplo de resposta:  
`{
"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJzdWIiOiJhZG1pbiIsImp0aSI6IjQ0ZDQ3NTRjLTE2N2ItNDAzOC05NzM3LTFmMzE1OTY0ZjE2YSIsImV4cCI6MTc0NzAwNTY1NSwiaXNzIjoiQXV0aFNlcnZpY2UiLCJhdWQiOiJBdXRoU2VydmljZUNsaWVudCJ9.E4FuYjFG7Te9kyUVWNU0sMT1_3wMnk2olfkAkPrusPU"
}`
- Adicionar o token ao swagger:
  - Acesse o swagger do ms transaction: `http://localhost/transaction/swagger/index.html`
  - No canto superior direito clique em "Authorize".
  - De posse do token, inclua o mesmo no campo de input com o prefixo "Bearer ", exemplo: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJzdWIiOiJhZG1pbiIsImp0aSI6IjQ0ZDQ3NTRjLTE2N2ItNDAzOC05NzM3LTFmMzE1OTY0ZjE2YSIsImV4cCI6MTc0NzAwNTY1NSwiaXNzIjoiQXV0aFNlcnZpY2UiLCJhdWQiOiJBdXRoU2VydmljZUNsaWVudCJ9.E4FuYjFG7Te9kyUVWNU0sMT1_3wMnk2olfkAkPrusPU`
  - Clique em "Authorize" e depois em "Close".
  - Agora pode fazer uso das rotas bloqueadas normalmente.
## Requisitos Funcionais:
- RF1: Registrar lançamentos (créditos e débitos) no fluxo de caixa.
  - Acessar swagger: `http://localhost/transaction/swagger/index.html`
  - Acesse a rota POST /Transaction > Try it out
  - Já tem modelo de payload setado, tente gravar datas diferentes, preferencialmente entre 2025-05-03 e 2025-05-07 (teste de performance busca datas nesse range).
  - Clique em "Execute" para executar a requisição.
  - Evidencie a transcação gerada no payload de retorno ou:
  - Vá na rota GET /Transaction > Try it out
  - Clique em "Execute" para executar a requisição.
  - Evidencie as transações geradas no payload de retorno.
- RF2: Atualizar lançamentos existentes.
  - Acessar swagger: `http://localhost/transaction/swagger/index.html`
  - Acesse a rota PUT /Transaction > Try it out.
  - De posse do Id de uma notificação existente, insira o mesmo no campo "id" do path params.
  - Modifique os campos disponíveis no payload exemplo.
  - Clique em "Execute" para executar a requisição.
  - Evidencie as transações geradas no payload de retorno ou faça uma requisição na rota GET /Transaction/{id}
- RF3: Excluir lançamentos.
  - Acessar swagger: `http://localhost/transaction/swagger/index.html`
  - Acesse a rota DELETE /Transaction > Try it out.
  - De posse do Id de uma notificação existente, insira o mesmo no campo "id" do path params.
  - Clique em "Execute" para executar a requisição.
  - Pode evidenciar a exclusão na rota GET /Transaction/{id} o resultado deve ser um 404 Not Found.
- RF4: Consultar lançamentos.
  - Acessar swagger: `http://localhost/transaction/swagger/index.html`
  - Acesse a rota GET /Transaction ou Transaction/{id} > Try it out.
  - Caso rota /Transaction pode ser configurado os inputs de paginação e ordenação.
  - Clique em "Execute" para executar a requisição.
  - Evidencie as transações no payload de retorno.
- RF5: Gerar relatório diário consolidado do fluxo de caixa.
  - Acessar swagger: `http://localhost/consolidation/swagger/index.html`
  - Acesse a rota GET /Consolidation/{date} > Try it out.
  - Insira alguma data no formato "yyyy-mm-dd" no campo "date" do path params, exemplo: `2025-05-07`.
  - Clique em "Execute" para executar a requisição.
  - Evidencie o resultado consolidado no payload de retorno.
- RF6: Sincronização entre os serviços de controle de lançamentos e consolidação diária.
  - Realize uma operação de transação, por exemplo, uma inclusão de nova transação no ms de transaction com uma data específica.
  - Realize uma busca de consolidado no ms de consolidation com a mesa data das inclusões.
  - Evidencie a mudança nos valores conforme a inclusão de novas transações.
## Requisitos Não Funcionais:
- RNF1: O serviço de consolidado diário recebe 50 requisições por segundo, com no máximo 5% de perda de requisições.
  - Com K6 instalado na máquina.
  - Acesse a pasta de tests/performanceTests do ms de consolidation.
  - Rode o seguinte comando para executar o teste de performance com K6: `k6 run ConsolidationPerformanceTest.js`.
  - Os testes irão rodar e ao final dá um resumo com os resultados, o último teste obteve o seguinte resultado:
  - ![image](https://github.com/user-attachments/assets/dff13b3a-4f89-4d9b-bef6-556b79847717)
  - Com a seguinte configuração para atender o requisito:
  - ![configuration](https://github.com/user-attachments/assets/1a81aa08-5db4-42a6-baca-9f0ace568828)
- RNF2: O serviço de controle de lançamento não deve ficar indisponível se o sistema de consolidado diário cair.
  - No docker, pare os dois containers "act-ms-consolidation-1" e "act-ms-consolidation-2".
  - Realize requisições através dos endpoints do ms transaction.
  - Evidencie que as requisições funcionam normalmente mesmo com os containers de consolidation pausados.
  - O console do RabbitMQ pode ser acessado evidenciando a fila acumulando mensagens, após, dê start no container novamente e veja a fila sendo processada e descarregando.
- RNF3: Implementar estratégias de recuperação de falhas.
  - Por conta do tempo do desafio, estratégias de Retry e Circuit Breaker não foram implementados.
- RNF4: Implementação de autenticação, autorização e criptografia.
  - Para este desafio, foi implementado serviço de auth,  que gera token JWT para uso em rotas protegidas.
  - Envidencie acessando rotas protegidas e recebendo unauthorize, após, desbloqueando as rotas com JWT gerado no ms auth.
  - Por conta do tempo, apenas o ms transaction possui rotas protegidas.
- RNF5: Garantir alta disponibilidade e escalabilidade para ambos os serviços.  
  - O balanceamento de carga com nginx pode ser evidenciado parando um dos containers, por exemplo, um do consolidation, enviando novas requisições, pode alterar dando start no que estava pausado e pausando o que estava funcional refazendo os testes.
  - Evidencie que o sistema distribui as cargas não impedindo sua funcionalidade, gerando alta disponibilidade, que pode ser configurado futuramente englobando auto scalling. 
- RNF6: Otimização de desempenho e confiabilidade.
  - No ms consolidation, foi incluido cache com redis com fallback para o banco, gerando otimização de desempenho nas requisições.
  - Nos ms's foram incluídos testes unitários para aumentar a confiabilidade de código.
- RNF7: Implementação de testes unitários.
  - Os dois ms's de transaction e consolidation possuem testes unitários.
  - Na raiz da solução pode ser executado com o comando: `dotnet test`
  - Isso rodará os testes e mostrará no terminal o sucesso dos resultados.
  - ![image](https://github.com/user-attachments/assets/df373547-5289-4c34-86e1-987fc3c2d1f9)  
- RNF8: Implementação de monitoramento e observabilidade.
  - Foi implementado prometheus para scrapping de dados no ms transaction. Futuramente pode ser implementado grafana para gerar os gráficos dos dados do prometheus.
  - Pode ser evidenciado realizando algumas interações nas rotas de transactions e acessar o console do prometheus realizando uma query dos dados.
  - Vá até: `http://localhost:9090/`
  - No campo de input insira uma query para métrica, por exemplo, receber o total de http status code 200: `http_requests_received_total{code="200"}`
  - Clique em execute e se tiver ao menos uma requisição com status 200 verá:
  - ![image](https://github.com/user-attachments/assets/9d5c62ce-2ddc-4989-bbbb-5a141a653e44)  
  - Pode ser visto em gráfico também, nesse caso teve 3 requisições:
  - ![image](https://github.com/user-attachments/assets/3816bba8-acb9-4ecb-b1c5-5c7112fab23a)  
- RNF9: Deve ser feito usando C#.
  - Todos os ms's foram realizados em C#.
# Possibilidade de melhorias:
- Configurar uma DLQ para mensagens mal formatadas e reprocessamento posterior.
- Caso tenha mais tipos de usuários, implementar autorização.
- CQRS caso aplicação passe a ter grande volumetria de chamadas, podendo separar operações de leitura e gravação de banco, escalando o mesmo conforme a necessidade, leitura horizontalmente e gravação verticalmente.
- Testes e2e automatizados com cypress ou selenium, acoplados a pipelines para garantir integridade funcional.
