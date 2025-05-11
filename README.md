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
- RNF3: Ter estratégias de recuperação de falhas.
- RNF4: Implementação de autenticação, autorização e criptografia.
- RNF5: Ter alta disponibilidade para ambos os serviços.
- RNF6: Ter documentação completa das decisões arquiteturais.
- RNF7: Ter otimização de desempenho, disponibilidade e confiabilidade.
- RNF8: Implementação de testes unitários.
- RNF9: Ter monitoramento e observabilidade.
- RNF10: Definição e estruturação dos domínios funcionais.
- RNF11: Criação de um Readme com instruções para execução local.


- Code First
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
