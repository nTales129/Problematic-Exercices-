# Encerramento da investigação ampla e prontidão de desenvolvimento

## Correção de fidelidade — 23/10/2026

As propostas autorais de rebalanceamento da especificação v0.1 foram revogadas. A regra recuperada Likes/Dislikes +1 será preservada. A implementação de uma visita foi autorizada pela instrução posterior do usuário. Consultar docs/DESIGN/GAMEPLAY_SPEC.md v0.2 e FIDELITY_MATRIX.md; afirmações históricas abaixo sobre pesos negativos, três dias ou espera de aprovação não são diretrizes vigentes.

## Estado atual após a correção

A primeira visita independente foi implementada com regras recuperadas e placeholders próprios. Os 26 testes EditMode e dois testes PlayMode passaram na Unity 6000.6.6f1. O fluxo dos controles foi exercitado por mouse virtual no Input System/EventSystem; capturas são renderizações offscreen do Canvas, não capturas do original. Ver `docs/DESIGN/FIRST_PLAYABLE_VALIDATION.md` para limites, evidências e build. Nenhum original/mod loader/PE_Tester foi executado.

## Registro histórico de encerramento da fase 3 — propostas substituídas

O restante preserva o fechamento anterior à correção do usuário. Afirmações sobre outras fórmulas, pesos negativos e espera de revisão abaixo descrevem a entrega histórica v0.1 e não orientam a implementação atual.

23/10/2026. A fase 3 encerrou a investigação ampla solicitada. Naquele momento, havia especificação para revisão e nenhum gameplay implementado.

## Descobertas que mudam o entendimento

| Resultado | Grau e consequência |
|---|---|
| Filtro diário olha currentLevel>0 | **CONFIRMED** no predicado; algoritmo composto de seleção/complemento **STRONGLY INFERRED**. Não era simplesmente filtro de clientes ainda não atendidos |
| Tutorial força um cliente específico | **CONFIRMED**: array de uma opção e inclusão/substituição na seleção diária. Não generalizar amostragem padrão para tutorial |
| Evento especial executa visita diretamente | **CONFIRMED**: cria cliente/habilidade, troca lógica do menu e chama MakeClientVisit sem escolha convencional. Não exige inserir IStageTurn no provedor |
| Visita narrativa usa outro caminho | **CONFIRMED** em evento de visitante: entrada, diálogo e saída, sem o caminho de avaliação padrão no corpo examinado |
| Interrupção de espera tem flag específica | **CONFIRMED**: jumpsAndInterrupts e inWait controlam Cancel; isImportant é outro campo, usado em outra política |
| Likes/Dislikes continuam convergindo | **CONFIRMED estaticamente** por leitura independente dos bytes de dois saltos para o mesmo incremento, além de enum/offsets originais. Intenção e validação em runtime **UNKNOWN** |
| Vídeos locais ficaram acessíveis | **OBSERVED**: 55 amostras de cinco vídeos, incluindo arraste da carta, transição, atividade, nota, fim diário e próximo dia. O bloqueio do navegador não impediu decodificação local |
| Composição visual parcialmente descoberta em runtime | **CONFIRMED**: pose reúne componentes por GetComponent/GetComponentsInChildren; assets conservam hierarquia, renderers, animators, bibliotecas e referências |
| Animação separada de duração do atendimento | **CONFIRMED**: clips curtos com loop e timespan/velocidade controlados pelo código; o vídeo não determina duração fixa do motor |

As fórmulas centrais da fase 2 permanecem documentadas em 03/12/13. Não foi necessário repetir sua investigação para criar uma especificação autoral. O novo jogo usa outras fórmulas e parâmetros deliberados; isso está explicitamente marcado em GAMEPLAY_SPEC.

## Correlação visual obtida

Todos os MP4 são 1280×720 e 30 fps constantes no track examinado. O vídeo principal tem 135,933 s. Amostras próximas a 39/39,5/40/40,5 s distinguem prévia, carta elevada e mudança para atividade; 46 s mostra nota; 50 s, retorno e opções de cliente; 110/114/118/122/126 s mostram fim, resultado, melhorias/recompensa e dia seguinte.

Esses são pontos amostrados, não precisão garantida de PTS nem medição de clique/latência. A apresentação de SETTINGS/SAVEFILE no vídeo histórico é observável, mas não valida nem reabre A/B/C. Não houve interação com a janela do original nesta fase.

Capturas e sequências autorizadas foram mantidas apenas na pesquisa privada. O pedido novo foi conferido após rejeição automática inicial que aplicava a restrição anterior. A tentativa de obter FFmpeg falhou por DNS; o decodificador Windows já instalado funcionou. Nenhum conteúdo visual foi movido para Assets, incorporado à produção ou publicado.

## Lacunas restantes e decisões de escopo

| Lacuna | Tratamento nesta entrega |
|---|---|
| Produtor efetivo de extraTurns não identificado | **UNKNOWN**, sem declarar infraestrutura morta. Primeiro escopo autoral não implementa visitas especiais |
| Totalidade de gatilhos/addons | **UNKNOWN**; dois eventos narrativos autorais em checkpoints, sem replicar catálogo proprietário |
| Intenção de Dislikes | **UNKNOWN**; regra negativa própria e teste de monotonicidade, sem corrigir o original ou copiar possível defeito |
| Preferências inline de amostra | Tentativa dirigida não produziu amostra adequada; confirmação vem dos metadados e bytes, não de dados serializados inventados |
| Bindings completos de todos os personagens/expressões | Amostra suficiente para separar responsabilidades; autoral usa actors/anchors/expressões próprios e fallback |
| Economia, desbloqueios e saves completos do original | Não necessários ao escopo independente; regras próprias explícitas de créditos, medalhas, galeria e DTO v1 |
| Balanceamento e diversão dos parâmetros propostos | **Não validados**; testar após aprovação e primeira sessão completa; atualizar dados/spec ao revisar |
| Compatibilidade de uma futura build Unity | **Não testada** nesta pesquisa; roadmap exige EditMode, PlayMode e build Windows independente |

Essas lacunas secundárias não justificam prolongar indefinidamente a engenharia reversa. Caso apareça um impedimento específico durante implementação, investigar apenas esse ponto, com novo escopo, preservando a separação autoral.

## Documentos produzidos

| Documento | Conteúdo |
|---|---|
| 16_ROSTER_AND_EVENTS.md | Filtros, tutorial, eventos, visita especial e revisão independente da anomalia |
| 17_VISUAL_SYSTEM.md | Amostra de composição, bibliotecas, clips/controllers e responsabilidades de código/assets |
| 18_REFERENCE_TIMELINE.md | Metadados, método local, timestamps amostrados, observações e limitações |
| 19_FINAL_RESEARCH_SUMMARY.md | Encerramento, lacunas, escopo e prontidão |
| docs/DESIGN/GAMEPLAY_SPEC.md | Referência normativa: sistemas, estados, regras autorais, fórmulas, erros e AC01–AC24 |
| docs/DESIGN/SYSTEM_ARCHITECTURE.md | Camadas, modelos, comandos, snapshots, apresentação e testes |
| docs/DESIGN/IMPLEMENTATION_ROADMAP.md | Doze etapas com dependências, saídas e forma de validar na Unity |

Os documentos anteriores receberam apontamentos de atualização, preservando os achados históricos. Artefatos de fase 3 ficam em ProblematicSubjects_Research/StaticPhase3; documentos anteriores à revisão são preservados em phase2_docs. A raiz do projeto não é um worktree Git nesta sessão; nenhum comando de mutação Git foi realizado.

## Verificação da entrega

O relatório privado `StaticPhase3/delivery_validation.json` confirma os sete documentos obrigatórios e os dez campos exigidos em cada um dos 19 sistemas da especificação. Foram conferidos fences, marcadores de conflito, espaços finais e referências de critérios do roadmap, sem problemas encontrados. Isso é validação documental, não teste da futura implementação.

Integridade: 47/47 arquivos da WorkingCopy mantêm tamanho e SHA-256 do baseline anterior, sem adições ou ausências; os cinco vídeos correspondem aos hashes registrados na análise local. Há fingerprint dos 62 arquivos atuais de Assets/Packages/ProjectSettings para a revisão final; nenhuma escrita de runtime foi feita pela tarefa. ZIP, saves, registro e Gym Discipline não foram usados como destinos. Não houve commit/push.

Para revisão, começar pela [especificação funcional](<C:/Users/Micro/Documents/GitHub/GameGymPjct/Problematic Exercise/docs/DESIGN/GAMEPLAY_SPEC.md>), depois [arquitetura proposta](<C:/Users/Micro/Documents/GitHub/GameGymPjct/Problematic Exercise/docs/DESIGN/SYSTEM_ARCHITECTURE.md>) e [roadmap](<C:/Users/Micro/Documents/GitHub/GameGymPjct/Problematic Exercise/docs/DESIGN/IMPLEMENTATION_ROADMAP.md>). A pesquisa complementar está em [elenco/eventos](<C:/Users/Micro/Documents/GitHub/GameGymPjct/Problematic Exercise/docs/REVERSE_ENGINEERING/16_ROSTER_AND_EVENTS.md>), [sistema visual](<C:/Users/Micro/Documents/GitHub/GameGymPjct/Problematic Exercise/docs/REVERSE_ENGINEERING/17_VISUAL_SYSTEM.md>) e [linha do tempo](<C:/Users/Micro/Documents/GitHub/GameGymPjct/Problematic Exercise/docs/REVERSE_ENGINEERING/18_REFERENCE_TIMELINE.md>).

## Prontidão e primeira etapa recomendada

**Pronto para revisão de especificação e, após aprovação, para implementação da primeira fatia.** O loop central, entradas/saídas e fórmulas autorais não dependem de preencher lacunas do original. A arquitetura prevê UI real, estados e apresentação separados, confirmação idempotente e save em checkpoints. Conteúdo inicial e tratamento de falhas estão delimitados.

Primeira etapa recomendada: um atendimento completo na Unity com placeholders autorais — iniciar, escolher atleta, três cartas, confirmar, atividade simples, nota e saída — usando o coordenador e modelos próprios. Depois integrar deck/recursos/dias, conforme roadmap. Não começar por extrair arte, reproduzir personagens ou ampliar lojas/campanha.

Antes de programar, o usuário deve revisar especialmente os pesos negativos, fórmulas de score/energia, cotas 3/4/5, alvo 150, economia e política de save. Essa revisão ainda não ocorreu; **nenhuma etapa de implementação foi iniciada**.
