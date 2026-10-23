# Matriz de fidelidade — v0.2

## Escopo vigente — fase 6

Esta tabela substitui os limites históricos de uma visita/dia1 nas seções anteriores de implementação.

| Área | Preservado / adaptação / lacuna |
|---|---|
| Campanha | C:5dias/cotas3/4/5/6/6; implementados pelo coordenador, sem menu automático entre dias |
| Visita e KO | Fórmulas v0.2 e aplicação única preservadas; concluir atividade/resultado/saída antes do KO local; KO encerra somente o dia |
| Resultados | S e saldos acumulados, clamp progresso500, pontos de presentes S+35×0; final pelo objetivo. Sem medalhas/overrides especiais |
| Catálogo | A:seis adultos originais; dia1 preserva Lia/Caio/Nara, cotas seguintes complementam com Inês/Raul/Bia |
| Níveis | C:predicado>0; I:complementação; U:LevelUp. A:prioridade fixa1/0, histórico e contadores próprios, sem fórmula inventada |
| Reset diário | C:reset de scores/gerenciador/passivos; U:contrato completo. A:novo deck, stamina100/modificador1; total/progresso/histórico persistem |
| Entre dias | C:resultado/limpeza/seção de melhorias/avanço; A:continuação direta explicitamente sem melhorias, sem botões falsos |
| Final | C:comparar progresso/meta sem override; A:texto próprio com dias/visitas/total/progresso/objetivo/resultado |
| Arraste | Mecânica mantida; diagnósticos de press/up/begin/release em desenvolvimento; aprovação Windows depende de execução registrada |
| Encerramento | Crash histórico preservado como intermitente não resolvido; stack com UIAutomationCore, sem causa própria demonstrada; nenhuma troca especulativa de Unity/pacotes |
| Pendentes | Recompra, tutorial/eventos, melhorias, presentes/recompensas, medalhas, save, loja, galeria, arte final |

Detalhes e evidências: `../DEVELOPMENT/PHASE6_CAMPAIGN.md`. Documentos abaixo registram evolução histórica; o escopo vigente é esta tabela e o adendo6 da especificação.

## Atualização de escopo — fase 5

Regras v0.2 preservadas; produção agora usa o dia 1 completo. As referências a “fatia” abaixo são históricas quando limitam a entrega a uma visita.

| Área | Implementação autorizada / limite |
|---|---|
| Menu | Nova sessão, Créditos e Sair reais; composição autoral. Introdução deixa de ser menu |
| Roster | Filtro nível>0 confirmado; shuffle/take/complemento inferidos. Fixture própria com três adultos nível1. Não inventar LevelUp; atendimentos completos são histórico separado |
| Continuidade | Deck/stamina/pontos compartilhados; até três visitas; uma opção quando só resta um cliente |
| Encerramento | Após visita confirmada completa e saída, KO local por stamina precede roster vazio; sem fontes especiais de KO nem eventos extras |
| Progresso | Soma dos resultados e clamp a500; pontos de presentes S+P*E (P35,E0), sem conversão/recompensas |
| Apresentação | Treinadora, cliente, sala, cartas e reação próprios; tempos/movimentos autorais e não medidos na referência |
| Input de arraste | Offset técnico usa pressPosition para tolerar primeiro movimento grande; limiar confirmado altura/3 preservado. Testes InputSystem passam; sky.drag na janela não confirmou e revisão manual continua pendente. Botão alternativo Confirmar validado na build |
| Escopo | Só dia1; cinco cotas preparadas como dados, sem executar dias2–5; recompra pendente |

23/10/2026. Atualizada antes da primeira implementação. C=CONFIRMED estático; V=OBSERVED em vídeo; I=STRONGLY INFERRED; U=UNKNOWN; A=ADAPTATION. Testes independentes não são confirmação dinâmica do original.

| Regra | Evidência | Alvo / primeira fatia / divergência |
|---|---|---|
| Perfil | V:04 início;C:configs fase1 | Detention5dias,3/4/5/6/6,meta500,P35,E0,false,threshold10; não misturar Gym. Só1visita agora |
| Atendimento | C:11,18d2fd0,18bb880 | Uma habilidade; entrada/mão/atividade/resultado/saída. Fim da fatia não é fim diário |
| Mão/deck | C/I:03,18e0705,18dfca0,18dfa80 | 3/clamp0..5 normal, topo/reciclagem/descarte integral/mão curta. Sem emergência nem unicidade global inventada |
| Tags requeridas | C:prioridade;U:desempate | Fixture sem requisitos; caminho especializado adiado |
| Recompra | C:18b1780,190ff20;U:inicial/reset | Decremento antes da compra; fora da fatia; remover2cargas inventadas |
| Confirmação | C:1913760;V:04~38–40,5s | Arraste centroY>=altura/3; baixo retorna; A:Overlay e botão alternativo, sem modal. U:Canvas original em todas resoluções |
| Likes/Dislikes | C:03/16 e revisão bruta fase3 | Ambos+1, preservados. Proteção contra penalizações inevitáveis é hipótese, não intenção comprovada |
| Preferência | C:18ac0e0 | Habilidade+tags; +1,+1,0,+25 e limites >20/positivo/zero/negativo completos |
| Nota/pontos | C:18abe10,18e3830 | 4raridades/6notas10/14/18/22/26/34. Sem5notas/tabela v0.1 |
| Stamina | C:18ea380,18edba0,18edbc0 | 10/25/40/55, float32/floor/clamp. A:inicial/max100 fixture, modificador1. Sem afirmar valor inicial recuperado |
| Custo/KO | C:03/11/12 | Cobrar início; completar visita mesmo a zero; sem teste de saldo suficiente. KO global/dia adiado |
| Resultado | C:18bc457 antes18bc953 | Creditado antes de Continuar; repetição bloqueada |
| Progressão do cliente na saída | C:11, fim de MakeClientVisit chama Client.LevelUp | Chamada recuperada deve orientar a etapa de progressão. A fatia termina após uma visita e não implementa níveis/reset de dias; nenhuma equivalência de progressão é alegada |
| Roster | C:16 nível>0/range/remoção;I:shuffle/take | A:3clientes ativos próprios; oferecer2ou3. Seleção diária/tutor específico adiados |
| RNG | U:helper/seed/distribuição completos | A:System.Random injetado/Fisher–Yates, seed reproduzível em testes. Não alegar ordem equivalente |
| Conteúdo | Exigência autoral;U:catálogo completo | A:3adultos/6exercícios/tags/preferências próprios. Disponibilidade fixture não prova desbloqueios |
| Atividade | C:17;V:04~40,5–46s | Interação de fim antes nota. A:formas/movimento/0,8s entrada-saída/Concluir, não duração original |
| Dias | C:12 | Cinco dias perfil, clientes/extraTurns/KO; adiado sem dia artificial de1cliente |
| Progresso | C:12 | S=ΣV;Tnovo=clamp(T+S,0,M);ganho=S+P*E; contrato futuro |
| Economia/save | U:recompensas/medalhas/persistência completas | Adiados; remover meta150/créditosfloor(S/25)+1 e economia inventada |
| Eventos | C:16 filas/players/flags;U:todos produtores | Sem eventos sintéticos nesta fatia; flags isImportant e jumpsAndInterrupts distintas |
| Código/arte | Exigência usuário | Originais; nenhuma extração/código descompilado em produção |

## Revogação v0.1

Três dias, cinco notas próprias, pontos4/7/11/16/22, custos12/20/30, peso negativo para configuração Dislikes, meta150, créditos/medalhas novos, duas recompras, emergência e zona retangular de drop não são contratos vigentes. Não testar monotonicidade negativa: contradiz a regra confirmada.

## Limites

Fatia valida regras locais/ordem da visita, não campanha/economia/tutorial/eventos. Adaptações técnicas e dados próprios continuam identificados como tais. Evidência de execução em FIRST_PLAYABLE_VALIDATION.md.
