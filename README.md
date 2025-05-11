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
- RNF6: Criar documentação completa das decisões arquiteturais.
- RNF7: Otimização de desempenho, disponibilidade e confiabilidade.
- RNF8: Implementação de testes unitários.
- RNF9: Implementação de monitoramento e observabilidade.
- RNF10: Definição e estruturação dos domínios funcionais.
- RNF11: Criação de um Readme com instruções para execução local.
- RNF12: Deve ser feito usando C#

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

- summaries para melhor documenta��o do c�digo.

- Incluir autorizações não apenas autenticações.
- ms-auth
	- Ajustar o design de código, criar rota de registro de usuários retirando mock.
- Verificar se as portas que os containers vão rodar nãoe estão ocupadas pela máquina que está executando.
- Caso terna

TODO:
- Incluir RabbitMQ e Redis no compose. OK
- Integrar rabbitmq na rota de criação e alteração e deleção. OK
- Implementar health check. OK
- Implementar middleware de erros. PENDENTE
- Implementar o serviço de consolidation integrando com redis e consulta ao banco como fallback. OK
- Implementar logs estruturados.PENDENTE
- Implementar testes unitários.OK
- Implementar testes com K6. OK
- Implementar containerização nos ms com dois para cada consolidation e transaction com nginx balanceando.OK
- Incluir Migrations no compose.
- Adicionar validators nos dtos ou classe de dominio
- Adicionais
	- Implementar prometheus e grafana gerando gráfico.
	- Integrar frontend.

TODO Filtrado:
- Colocar tratamento dos dados de entrada nos ms.
- MIddleware de exceptions. OK
- Colocar exemplo no swagger para ms auth. com user admin e pass 123456. OK
- Aplicar os logs estruturados (Prioridade.) OK 
- Migrations, ver forma de fazer no compose. NÃO FOI POSSIVEL.
- Traçar tópicos de apresentação em vídeo se basear no documento.
- Verificar todos os arquivos (Anotações desnecessárias, refs desnecessárias.)

Planos futuros
- Configurar uma deadQueue para mensagens mal formatadas e reprocessamento posterior.
- Caso tenha mais tipos de usuários, implementar autorização.
