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
- Colocar exemplo no swagger para ms auth. com user admin e pass 123456.
- Aplicar os logs estruturados (Prioridade.)
- Migrations, ver forma de fazer no compose.
- Traçar tópicos de apresentação em vídeo se basear no documento.
- Verificar todos os arquivos (Anotações desnecessárias, refs desnecessárias.)
- Mencionar na doc sobre case sensitive das rotas quecomeçam com maiúscula.

Planos futuros
- Configurar uma deadQueue para mensagens mal formatadas e reprocessamento posterior.
- Caso tenha mais tipos de usuários, implementar autorização.