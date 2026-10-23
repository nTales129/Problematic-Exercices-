# Resumo da etapa 2 — comportamento recuperado

## Atualização de status — fase 3

A fase 3 foi concluída e está resumida em 19_FINAL_RESEARCH_SUMMARY. As lacunas e recomendação abaixo representam o fechamento histórico da fase 2. A instrução posterior do usuário autorizou a primeira visita com fidelidade às regras recuperadas; consultar GAMEPLAY_SPEC v0.2, FIDELITY_MATRIX e FIRST_PLAYABLE_VALIDATION para o estado atual.

## Registro anterior preservado

23/10/2026. A etapa avançou de contratos para análise nativa dirigida. O bloco A foi investigado em toda sua cadeia central; o bloco B produziu resultados substanciais; o bloco C ficou parcial. Não houve execução do jogo, retomada do modding ou implementação autoral.

## Respostas aos critérios de sucesso

| Questão | Resposta sustentada | Grau / limite |
|---|---|---|
| A. Sequência real do turno | Instância nova parte do marcador Prepare; primeira chamada incrementa para ChooseClient; próxima para Client; conclusão da visita escreve Done | **CONFIRMED** nos branches; estado zero inicial de instância nova **STRONGLY INFERRED** |
| B. Escolha e confirmação | UI separa seleção, arraste e escolha. O caminho de soltura confirma acima do limiar altura/3, emite ID e conclui a espera; a mão resolve a habilidade | **CONFIRMED** no código; sem teste de mouse |
| C. Consumo/descarte | Ao concluir a espera, a mão é limpa e todas as compras vão ao descarte. Reposição recicla descarte; reroll consome carga; uso de habilidade possui listeners de stamina | Descarte/cargas **CONFIRMED**; encadeamento integral dos listeners **STRONGLY INFERRED** |
| D. Preferências | Junta correspondências por habilidade e tag e acumula pesos. Likes e Dislikes convergem para +1; Favorite soma 25; limiar de favorito é >20 | **CONFIRMED** no corpo; intenção da anomalia **UNKNOWN** |
| E. Nota | Favorite produz W; demais combinam base de raridade com ajuste de preferência. Pontos por D/C/B/A/S/W = 10/14/18/22/26/34 | **CONFIRMED**, entradas válidas e caminho local |
| F. Fim da visita | Uma escolha/execução padrão, eventos após habilidade, transições de saída, diálogo de saída condicional, reset/limpeza de overrides, LevelUp, fase de fim | **CONFIRMED** no fluxo normal; extensões/cancelamento total **UNKNOWN** |
| G. Fim do dia | KO pode interromper entre etapas; flag true pula só um teste intermediário. Turno com KO encerra dia; sem clientes e extras, provedor esgota | **CONFIRMED**, precedência local analisada |
| H. Progressão | Scores diários alimentam redução de pontos; aplicação satura progresso e acrescenta pontos de presentes. Último dia segue para resultado final, com override ou comparação ao máximo | **CONFIRMED** no laço da redução, aplicação e branches do final |

## Fórmulas e parâmetros identificados

**CONFIRMED:** tamanho padrão da mão 3; compra normal usa clamp 0–5; quantidade explícita tem caminho diferente. Reroll decrementa cargas, não um custo de stamina demonstrado nesse handler.

**CONFIRMED:** agregação de preferência observada `t=1×Likes + 1×Dislikes + 25×Favorite`, com Ambivalent contribuindo zero. A preferência agregada é Favorite se t>20, Likes se 0<t≤20 e Ambivalent se t=0; existe ramo negativo para Dislikes, sem contribuição negativa encontrada para entradas normais. Essa particularidade foi preservada sem “corrigir” o binário.

**CONFIRMED:** nota g=5 para Favorite; caso contrário g=r+1+ajuste, com ajustes +1/-1/0 para Likes/Dislikes/Ambivalent. D..W usa a tabela de pontos, não o valor do enum como pontuação. Não há RNG ou arredondamento nessa rotina.

**CONFIRMED:** soma diária S dos pontos das notas; progresso ganha S, pontos de presentes ganham S+P×E. **CONFIRMED:** aplicação do progresso usa clamp; os valores de animação de presentes são divisões float por P. Isso não recupera toda a concessão e abertura de recompensas.

**CONFIRMED:** custo-base de stamina por raridade 10/25/40/55; modificador reduzido por percentual/100. A revisão do auxiliar revelou floor do delta negativo após multiplicação float32, equivalente ao teto do gasto positivo no domínio normal. Não houve execução do código original.

## O que permanece desconhecido

- **UNKNOWN:** razão da convergência Likes/Dislikes e comportamento validado em runtime. Não foi executado teste dinâmico.
- **UNKNOWN:** unicidade global de habilidades/IDs, distribuição exata de compra, todos os filtros do roster e clientes obrigatórios fora do caminho padrão.
- **UNKNOWN:** mapa completo de passivos, KO por estágio, revive, overrides, turnos especiais e gatilhos de visitantes.
- **UNKNOWN:** todas as garantias de limpeza em cancelamento, pausa e exceção.
- **UNKNOWN:** fórmula integral de medalhas, abertura de presentes, desbloqueios e persistência permanente.
- **UNKNOWN:** timestamps e correlação visual. O navegador rejeitou a abertura do MP4 local; não foi feito contorno nem exportação de imagens.

## Documentos e evidências

Atualizados: 01_ARCHITECTURE, 02_GAMEPLAY_LOOP e 09_UNCERTAINTIES. Criados: 03_CHOICES_AND_SCORING, 11_TURN_STATE_MACHINE, 12_DAY_AND_PROGRESSION, 13_NATIVE_ANALYSIS, 14_VIDEO_CORRELATION e este 15_PHASE2_SUMMARY.

O documento 13 fornece índices originais, tokens originais e de wrapper, RVAs, VAs, offsets e branches decisivos. Os artefatos em `ProblematicSubjects_Research/StaticPhase2` permitem revisar as conclusões sem copiar código ou conteúdo para Assets. Intervalos decodificados são candidatos; sua contagem não representa quantidade de métodos compreendidos.

**CONFIRMED:** conferência de integridade encontrou 47/47 arquivos da WorkingCopy com tamanho e SHA-256 iguais ao baseline, sem adições ou ausências. ZIP, saves, registro e Gym Discipline não foram alterados. Não houve commit/push. No projeto autoral foram modificados somente documentos.

## Implicações e próxima etapa recomendada

**ORIGINAL DESIGN DECISION:** há conhecimento suficiente para escrever uma especificação própria do ciclo dia → visita → escolha → atividade → avaliação → resultado, distinguindo política de domínio e apresentação assíncrona. Personagens, textos, identidade visual, código e parâmetros devem ser autorais; a investigação não é um projeto Unity recuperado para reutilização.

Recomendação: fazer uma revisão técnica dirigida dos filtros do roster, produtores de turnos extras e composição de addons; correlacionar 04_GameplayRun em um player permitido. Em seguida, elaborar a especificação independente com decisões explícitas para preferências negativas, mão vazia, recursos e economia. Não reabrir uma investigação genérica da engine nem repetir o inventário inicial.

Essa recomendação não foi iniciada automaticamente. Nenhuma mecânica de Problematic Exercise foi implementada.
