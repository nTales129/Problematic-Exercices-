# Incertezas após a investigação nativa da etapa 2

## Atualização de status — fase 3

Na fase 3, o filtro level>0, o cliente obrigatório do tutorial, uma visita direta por evento e a correlação dos vídeos foram investigados. Portanto essas linhas abaixo descrevem pendências históricas da fase 2, não o status final. Permanecem abertas totalidade de addons/gatilhos, produtor de extraTurns e dados completos de preferências. Status final no documento 19.

## Registro anterior preservado

23/10/2026. Esta revisão remove as lacunas resolvidas da etapa 1. Evidências e limites metodológicos estão no documento 13. Ausência de validação dinâmica não transforma a análise nativa em teste de execução.

## Questões resolvidas ou reduzidas

| Questão antiga | Estado atual |
|---|---|
| Não há correspondência com corpos nativos | **CONFIRMED:** cache de RVAs foi reutilizado e métodos centrais conferidos nos metadados originais; corpos e continuações foram examinados |
| Ordem do turno desconhecida | **CONFIRMED:** incremento antes da etapa, seleção, visita e Done; inicial zero de nova instância **STRONGLY INFERRED** |
| Repetição de ações por visita desconhecida | **CONFIRMED:** o helper padrão tem uma escolha e uma execução; extensão por evento/override continua **UNKNOWN** |
| Entrada, diálogo, habilidade e saída sem ordem | **CONFIRMED:** sequência normal recuperada; algumas transições visuais executam em WhenAll |
| Tamanho inicial da mão desconhecido | **CONFIRMED:** 3 no construtor; compra normal limitada a 0–5; compras explícitas não têm esse mesmo clamp |
| Reroll sem custo conhecido | **CONFIRMED:** decrementa cargas; reposição devolve compras ao descarte |
| Fórmula de notas totalmente desconhecida | **CONFIRMED:** agregação observada, bases de raridade, ajustes e pontos por nota; anomalia de preferências exige cautela |
| Encerramento diário sem predicado | **CONFIRMED:** KO entre etapas conforme flag e no resultado final; esgotamento de clientes e extras |
| Ordem de dias/upgrades/final desconhecida | **CONFIRMED:** LevelFlow distingue último dia e dias intermediários; cálculo final permite override |
| Uso dos parâmetros de presentes/progresso desconhecido | **CONFIRMED:** aplicação/clamp/divisões e redução diária como soma, após seguir o laço e o callback |

**CONFIRMED na revisão final:** a redução começa em zero e o callback acrescenta GradeValue; o auxiliar de stamina implementa floor do delta negativo no domínio finito normal. Essas duas questões deixaram de ser lacunas; documento 12 e fonte N9 registram os detalhes.

## Lacunas prioritárias que permanecem

| Prioridade | Pergunta aberta | Evidência já disponível / próximo procedimento estático específico |
|---|---|---|
| P0 | Por que Dislikes soma +1? | Dois branches convergem no mesmo incremento; enum e offsets originais conferidos. Revisar representação/configurações relevantes antes de atribuir intenção; não corrigir a regra por plausibilidade |
| P1 | Custos fora do domínio normal | Floor do delta negativo confirmado após análise de 3d3868; faltam política global de modificadores negativos/excessivos, não finitos e overflow |
| P1 | Filtros do roster e clientes obrigatórios | Caminhos diário/aleatório/tutorial distintos; resolver predicados e quais overrides cada estágio instala |
| P1 | Duplicatas, IDs e mão sem opções | Compra pode devolver menos cartas; FindSkill compara ID. Falta comprovar unicidade global, todas as entradas de compra e fallback de UI |
| P1 | Recuperação de cancelamento | Tokens, exceções e limpeza normal vistos; não demonstrado finally abrangente para toda a cadeia ou restauração visual após interrupção |
| P1 | Extensões que repetem visita ou criam turno extra | Provedor prioriza extras; produtores e fluxos de cada turno especial não completamente analisados |
| P1 | Composição integral das fontes de KO | Player usa koSources; handlers comum e Gym são diferentes. Falta fechar todos os addons/revive e interações de eventos |
| P2 | Presentes e medalhas | Ganho de pontos e progresso localizados; conversão em aberturas, recompensa concreta e fórmula completa de medalhas pendentes |
| P2 | Visitantes e eventos | Filas por fase, dispatch e filtragem por importância vistos; mapa completo de gatilhos/efeitos não concluído |
| P2 | Persistência | Snapshot de sessão distinto do progresso permanente e chamadas de criação vistas; não examinados saves pessoais nem toda a política de gravação/migração |
| P2 | Vídeos | Nenhum acontecimento observado/timestamp registrado. Abertura file: rejeitada pela política do navegador; roteiro no documento 14 |

## Limites que continuam válidos

**CONFIRMED:** wrappers não são implementações originais. O cache ajudou a localizar endereços, mas a evidência de comportamento veio de instruções e metadados originais. Tokens do wrapper, índices originais, RVA, VA e offset não são intercambiáveis.

**CONFIRMED:** fragmentos .pdata, inlining, código compartilhado e tabelas de saltos tornam enganosa uma leitura linear sem seguir branches. O primeiro alias de um endereço compartilhado não identifica o destino semântico. Os intervalos candidatos decodificados não foram todos analisados.

**UNKNOWN:** bootstrap integral, todas as referências externas de assets, recuperação dos MonoBehaviours ilegíveis identificados historicamente, regras de loja/galeria e transições detalhadas de animação. Esses sistemas não foram promovidos a compreendidos por proximidade com os métodos centrais.

Não foram exportados diálogos, imagens ou cenas. Não houve execução do jogo, loader, conta de testes, A/B/C, modding, implementação, commit/push ou alteração do Gym Discipline. Os 47 arquivos da WorkingCopy passaram na comparação de integridade; evidências anteriores foram preservadas, incluindo cópias dos documentos da etapa 1 na pesquisa privada.

## Próxima etapa recomendada

Revisão estática estreita dos filtros, produtores de turnos extras e composição de addons; correlação do vídeo prioritário em player permitido; depois especificação autoral que diferencie comportamento confirmado e decisões novas. **ORIGINAL DESIGN DECISION:** definir explicitamente preferências negativas, mão vazia, recursos, economia, IDs e conteúdo próprios. A investigação não autoriza importar código ou conteúdo proprietário para o runtime.
