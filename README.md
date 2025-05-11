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
#### MS Auth (Authentication Service)
- Função: Emite e valida tokens JWT.
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

# Detalhamento dos serviços, estruturas e componentes.
## MS Auth
- Design de código: Em camadas.
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
