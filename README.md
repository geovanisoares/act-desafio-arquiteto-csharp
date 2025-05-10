- Code First
- summaries para melhor documenta��o do c�digo.

- Incluir autorizações não apenas autenticações.
- ms-auth
	- Ajustar o design de código, criar rota de registro de usuários retirando mock.
- Verificar se as portas que os containers vão rodar nãoe estão ocupadas pela máquina que está executando.

TODO:
- Incluir RabbitMQ e Redis no compose. OK
- Integrar rabbitmq na rota de criação e alteração e deleção. OK
- Implementar health check. OK
- Implementar middleware de erros. PENDENTE
- Implementar o serviço de consolidation integrando com redis e consulta ao banco como fallback. OK
- Implementar logs estruturados.PENDENTE
- Implementar testes unitários.OK
- Implementar testes com K6. PENDENTE
- Implementar containerização nos ms com dois para cada consolidation e transaction com nginx balanceando.OK
- Adicionais
	- Implementar prometheus e grafana gerando gráfico.
	- Integrar frontend.

Planos futuros
- Configurar uma deadQueue para mensagens mal formatadas e reprocessamento posterior.