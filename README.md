- Code First
- summaries para melhor documenta��o do c�digo.

- Incluir autorizações não apenas autenticações.
- ms-auth
	- Ajustar o design de código, criar rota de registro de usuários retirando mock.


TODO:
- Incluir RabbitMQ e Redis no compose.
- Integrar rabbitmq na rota de criação e alteração e deleção.
- Implementar health check.
- Implementar middleware de erros
- Implementar o serviço de consolidation integrando com redis e consulta ao banco como fallback.
- Implementar logs estruturados.
- Implementar testes unitários.
- Implementar testes com K6.
- Adicionais
	- Implementar prometheus e grafana gerando gráfico.
	- Integrar frontend.