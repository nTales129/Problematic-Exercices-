# Problematic Exercise — especificação funcional

## Adendo normativo — fase 5: primeiro dia completo

A fase 5 autoriza expandir a primeira visita para **somente o dia 1**, com cota de três clientes. As fórmulas v0.2 abaixo permanecem vigentes. As limitações históricas de uma visita, introdução como entrada e progressão diária adiada são substituídas por este adendo; a cena FirstVisit permanece apenas como regressão dos 28 testes anteriores.

Fluxo de produção: menu principal (Nova sessão / Créditos / Sair) → preparação do dia 1 → roster → entrada e diálogo → mão → confirmação → atividade → reação e resultado → saída → checagem diária → próximo roster ou resultado diário. Não voltar à introdução entre visitas. Retorno ao menu apenas na conclusão da demonstração; nova sessão recria os dados, reutilizando a mesma hierarquia de UI.

Compartilhar um deck e um saldo de stamina/pontos entre visitas. Aplicar score uma vez antes de Continuar; registrar histórico por cliente/exercício/nota/pontos. Remover cliente ao selecionar, sem reposição diária. Último cliente pode oferecer uma única opção: a amostragem de 2 ou 3 é limitada pelo número disponível. Dia termina após a saída quando stamina=0 (precedência local) ou roster vazio. Sem fontes extras de KO, turnos especiais ou eventos nesta etapa.

Preparar roster diário com filtro de nível>0, embaralhar/tomar cota e complementar com clientes restantes se necessário, conforme inferência documentada. Lia, Caio e Nara têm nível inicial1 como dado autoral provisório. A chamada original LevelUp existe, mas sua fórmula continua UNKNOWN; não inventar incremento de nível. Registrar atendimentos completos separadamente e manter essa lacuna explícita.

Perfil imutável contém cotas `[3,4,5,6,6]`, mas somente índice0 pode iniciar. Stamina inicial100 permanece provisória. Resultado diário apresenta clientes, notas, pontos individuais, soma diária, acumulado, motivo e progresso `clamp(T+S,0,500)`. Calcular também saldo em pontos de presentes `S+35*0`, sem abrir presentes, criar recompensas, loja, medalhas ou economia permanente. Sem botão de dia 2.

A apresentação ganha treinadora própria, cliente com entrada/saída, sala persistente, HUD compacto, cartas hierarquizadas e reação antes do próximo roster. A chegada termina antes da fala, e a mão só aparece após a fala: placeholders de0,8s de entrada e0,6s de diálogo, com saída0,8s e reação0,4s. Tempos e movimentos geométricos são adaptações visuais, não medidas do original. Recompra, eventos, tutorial, save, galeria, campanha restante e arte final seguem pendentes. Ver `docs/DEVELOPMENT/PHASE5_FIRST_DAY.md` para evidências atuais; o restante desta especificação conserva a referência de regras e o registro da primeira fatia.

Versão 0.2 — 23/10/2026. Correção de fidelidade autorizada pelo usuário, seguida da implementação **somente do primeiro atendimento**. Substitui integralmente v0.1, preservada na pesquisa privada `FirstSlice/design_v0.1`. Rebalanceamento anterior não aprovado.

## Contrato e proveniência

Código, personagens adultos, academia, textos, arte e identidade próprios. Prioridade: equivalência funcional das mecânicas recuperadas. CONFIRMED refere-se à análise estática; OBSERVED a vídeo; STRONGLY INFERRED não equivale a execução; ADAPTATION indica escolha independente. Testar o novo jogo não confirma dinamicamente o original.

Fontes locais em docs/REVERSE_ENGINEERING: 03_CHOICES_AND_SCORING, 11_TURN_STATE_MACHINE, 12_DAY_AND_PROGRESSION, 13_NATIVE_ANALYSIS, 16_ROSTER_AND_EVENTS, 17_VISUAL_SYSTEM e 18_REFERENCE_TIMELINE. Consultar [FIDELITY_MATRIX](FIDELITY_MATRIX.md) para diferenças. Nenhum helper, binário, código descompilado ou asset extraído entra em Assets.

**Perfil de referência:** o início de 04_GameplayRun mostra **Detention**, não Gym. Configuração recuperada: cinco dias `[3,4,5,6,6]`, hasRandomClientsEveryday=false, meta 500, pontos por presente 35, extras 0, pleaseThreshold 10. Não misturar com parâmetros de Gym por causa da nova ambientação. A fatia contém uma visita, não simula fim de dia completo.

## Turno e visita

Uma habilidade por visita padrão. Turno original Prepare0/ChooseClient1/Client2/Done3; Prepare não é uma atividade executável adicional. Fluxo independente:

`Início → Seleção de cliente → Entrada/diálogo → Três habilidades → Escolha/prévia → Confirmação → Atividade → Avaliação → Resultado → Saída → Fatia concluída`.

Confirmar resolve habilidade da mão, limpa toda mão e descarta todas as compras **antes** de executar a referência escolhida. Uso/custo ocorre ao começar a habilidade. Conclusão da atividade calcula e registra score **antes** de esperar Continuar. Continuar libera saída, sem reaplicar score. Repetições são rejeitadas por fase. Uma visita confirmada termina mesmo que custo zere stamina. Não inventar teste de saldo suficiente nem segunda escolha na visita.

## Mão, deck e recompra

Inicial 3, compra normal limita tamanho a 0..5. Compra explícita não recebe necessariamente esse teto. Comprar do início da pilha; vazia, transferir descarte, limpar e embaralhar. Ambas vazias retornam mão curta. Descartar todas compradas, não apenas a escolhida. Não inventar exercício de emergência nem afirmar unicidade global de IDs do original. Catálogo próprio desta fixture é único para resolução inequívoca.

Tags requeridas são processadas primeiro; desempate completo UNKNOWN. Fixture sem requisitos, sem implementar caminho fictício. Recompra consome carga antes da nova mão; UI disponível com cargas>0. Inicial/reset por estágio UNKNOWN: não fixar duas cargas e não oferecer recompra nesta fatia.

## Preferências, avaliação e pontos

Reunir preferência específica da habilidade e cada correspondência de tag, sem prioridade exclusiva da específica. Acumulador inteiro t: **Likes+1, Dislikes+1, Ambivalent0, Favorite+25**. Classificação: t>20 Favorite; 0<t<=20 Likes; t=0 Ambivalent; t<0 Dislikes. Preservar ramo negativo, embora contribuições válidas usuais não o alcancem. Dados autorais usam tags distintas; não introduzir deduplicação silenciosa de contribuições.

Convergência Likes/Dislikes confirmada independentemente por bytes na fase 3. Evitar penalizações inevitáveis de clientes difíceis é **hipótese de design**, não intenção comprovada. Não classificar automaticamente como bug nem transformar Dislikes em contribuição negativa.

Para raridade r0..3: Favorite produz W=5, antes de validar raridade; demais categorias usam g=r+1+ajuste, Likes+1/Dislikes-1/Ambivalent0. O ajuste da categoria agregada Dislikes não torna negativa uma configuração Dislikes isolada. Sem RNG/rounding/multiplicador de upgrade no cálculo.

| g | Nota | Pontos |
|---|---|---:|
| 0 | D | 10 |
| 1 | C | 14 |
| 2 | B | 18 |
| 3 | A | 22 |
| 4 | S | 26 |
| 5 | W | 34 |

Exemplos: r0 sem correspondência C14; r0 com Likes ou Dislikes B18; Favorite W34; t20 Likes e t21 Favorite. r0 com categoria agregada Dislikes D10 é caso do calculador, não resultado normal de uma configuração Dislikes.

## Stamina

Raridades 0/1/2/3 custam **10/25/40/55**. Modificador float32 m; redução aplica m-=percentual/100f. Delta normal=floor(float32(-custo*m)), convertido a inteiro. Ex.: 25 e 0,7f → -18. Penalidade direta ignora modificador/rounding. Atualização satura em[0,max]; atual0 sem revive não muda; infinito não muda. Saldo menor que custo não impede habilidade e pode chegar a zero. Composição completa das fontes de KO UNKNOWN.

Fatia usa caminho comum do jogador, não handler específico de personagens de Gym. **100 inicial/máximo é adaptação de fixture**, não valor recuperado. Modificador1, sem passivo/infinito/penalidade no conteúdo desta etapa.

## Contratos futuros — não implementar agora

Roster não aleatório: predicado currentLevel>0 confirmado; selecionar/embaralhar/completar do restante STRONGLY INFERRED. Opções: chamada Random.Range(2,4) confirmada; shuffle/take inferidos. Remover escolhido dos disponíveis. Tutorial tem override que força cliente específico, não generalizar. Fatia padrão sem tutorial.

O caminho original chama Client.LevelUp ao finalizar a visita, após restauração do mundo. Essa chamada confirmada deve ser preservada na futura integração de níveis/roster; a fatia não implementa progressão do cliente nem afirma equivalência nesse sistema.

Turnos extras FIFO precedem normal; produtor UNKNOWN. Dia continua com cliente OU turno extra. KO do turno encerra antes de nova consulta ao provedor, com precedência local sobre esgotamento simultâneo. Flag de continuidade evita apenas checagem intermediária, não dá imunidade. Esgotamento OutOfClients; KO OutOfStamina. Dias indexados; intermediários têm resultado/limpeza/melhorias, último final.

S=Σpontos(nota); Tnovo=clamp(T+S,0,M); ganho de pontos de presentes=S+P*E. Referência M500/P35/E0. Frações de apresentação são floats, não quantidade inteira de presentes abertos. Conversão/consumo/recompensas/medalhas UNKNOWN: não inventar economia. Final sem override usa T>=M. Overrides completos/save UNKNOWN.

Filas General0/SkillPlayEnded1/EndOfDay2. Eventos após atividade precedem restauração/saída. Visita especial pode chamar atendimento diretamente com lógica substituída; visitante narrativo não implica score. jumpsAndInterrupts && inWait interrompe espera; isImportant é outro campo. Nenhum evento sintético nesta etapa.

## Conteúdo e adaptações mínimas

Três adultos próprios: Lia (mobilidade Likes, força Dislikes), Caio (força Likes, cardio Dislikes), Nara (cardio Favorite, mobilidade Ambivalent). Fixture já ativa; amostrar2ou3 opções, sem afirmar catálogo/seleção diária completos. Seis cartas próprias: Mobilidade(r0,mobilidade), Postura(r0,controle), Remada(r1,força), Equilíbrio(r1,mobilidade/controle), Circuito(r2,cardio), Série avançada(r3,força/cardio). Disponibilidade é fixture, não desbloqueio recuperado.

RNG original UNKNOWN: System.Random injetado/Fisher–Yates é adaptação técnica, não ordem equivalente. Sem streams/save novos. Apresentação uGUI Overlay, referência 1280×720, escala com tela. Clique seleciona para prévia; carta arrastada, ao soltar com **centroY>=Screen.height/3**, confirma; abaixo retorna. Overlay fornece coordenada de tela explícita; correspondência completa com Canvas original UNKNOWN. Botão Confirmar é alternativa de acessibilidade ADAPTATION, sem modal adicional. Entrada/saída0,8s e formas em movimento são placeholders, não tempos originais. Botão Concluir exercício representa interação de fim, sem inferir duração pelo loop animado.

## Sistemas — dez campos por contrato

C=CONFIRMED; V=OBSERVED; I=STRONGLY INFERRED; A=ADAPTATION; U=UNKNOWN. Fora da fatia significa contrato documental, não funcionalidade entregue.

### S01 Início

**Responsabilidade:** Criar fixture. Objetivo: Iniciar.

**Dados:** Fixture em memória, seed17, catálogo próprio.

**Estados:** Início/seleção

**Entradas:** Iniciar

**Saídas:** Roster

**Regras:** A:memória

**Dependências:** Catálogo

**Visual:** Painel inicial e botão Iniciar atendimento.

**Erros:** Inválido bloqueia

**Aceitação:** AC01

### S02 Turno

**Responsabilidade:** Ordenar. Objetivo: Uma visita.

**Dados:** Fase, cliente atual, exercício e resultado da visita.

**Estados:** Seleção/cliente/fim

**Entradas:** Comandos

**Saídas:** Conclusão

**Regras:** C:uma habilidade

**Dependências:** Visita

**Visual:** Cabeçalho e painéis derivados da fase, sem fórmula na UI.

**Erros:** Repetição rejeitada

**Aceitação:** AC02

### S03 Roster

**Responsabilidade:** Amostra/remoção. Objetivo: Opções.

**Dados:** Clientes disponíveis e subconjunto oferecido, IDs próprios.

**Estados:** Disponível/escolhido

**Entradas:** Fixture/RNG

**Saídas:** 2ou3

**Regras:** C/I:range/remoção;A:catálogo

**Dependências:** RNG

**Visual:** Dois ou três botões de cliente.

**Erros:** ID não oferecido rejeitado

**Aceitação:** AC03

### S04 Entrada

**Responsabilidade:** Transição. Objetivo: Apresentar.

**Dados:** Cliente escolhido, progresso visual de entrada.

**Estados:** Entrada

**Entradas:** Cliente

**Saídas:** Mão

**Regras:** C/V:ordem;A:0,8s

**Dependências:** View

**Visual:** Ator próprio entra em0,8s e diálogo autoral.

**Erros:** Input prematuro rejeitado

**Aceitação:** AC04

### S05 Deck

**Responsabilidade:** Comprar/reciclar. Objetivo: Pilhas.

**Dados:** Compra, descarte, cartas compradas e handSize3.

**Estados:** Compra/descarte/compradas

**Entradas:** Cartas/RNG

**Saídas:** Mão

**Regras:** C:topo/mão curta

**Dependências:** Catálogo

**Visual:** Cartas expostas; pilhas são modelo, sem tela adicional.

**Erros:** Quantidade negativa inválida

**Aceitação:** AC05

### S06 Mão

**Responsabilidade:** Prévia. Objetivo: Três opções.

**Dados:** Lista de exercícios e referência selecionada.

**Estados:** Decisão

**Entradas:** Compra/ID

**Saídas:** Seleção

**Regras:** C:3/clamp0..5

**Dependências:** Deck

**Visual:** Três cartas, realce da seleção e prévia de custo.

**Erros:** ID fora rejeitado

**Aceitação:** AC06

### S07 Confirmar

**Responsabilidade:** Descartar/cobrar. Objetivo: Comprometer.

**Dados:** ID pertencente à mão e referência escolhida.

**Estados:** Decisão/atividade

**Entradas:** ID/drag/botão

**Saídas:** Atividade

**Regras:** C:integral;A:botão

**Dependências:** Visita/UI

**Visual:** Arraste com limiar de tela; botão alternativo documentado.

**Erros:** Duplo input sem efeito

**Aceitação:** AC07

### S08 Preferência

**Responsabilidade:** t/categoria. Objetivo: Agregar.

**Dados:** Dicionários por habilidade/tag e acumulador int32.

**Estados:** Sem mutação

**Entradas:** Cliente/carta

**Saídas:** Preferência

**Regras:** C:+1,+1,0,+25

**Dependências:** Dados

**Visual:** Não expor enum técnico como diálogo de personagem.

**Erros:** Enum inválido falha

**Aceitação:** AC08

### S09 Nota

**Responsabilidade:** Raridade/categoria. Objetivo: Avaliar.

**Dados:** Raridade0..3, categoria agregada, enum de nota e tabela.

**Estados:** Sem mutação

**Entradas:** r/p

**Saídas:** D..W

**Regras:** C:fórmula/tabela

**Dependências:** Preferências

**Visual:** Nota D/C/B/A/S/W no resultado.

**Erros:** Raridade inválida falha sem Favorite

**Aceitação:** AC09

### S10 Stamina

**Responsabilidade:** Delta/clamp. Objetivo: Consumir.

**Dados:** Atual, máximo, delta, modificador float32, flags de mudança.

**Estados:** Positiva/zero

**Entradas:** Custo/m

**Saídas:** Atual

**Regras:** C:10/25/40/55/floor;A:max100

**Dependências:** Visita

**Visual:** Contador stamina atual/máximo.

**Erros:** Config inválida bloqueia

**Aceitação:** AC10

### S11 Atividade

**Responsabilidade:** Movimento/fim. Objetivo: Exercício.

**Dados:** Exercício escolhido, ator geométrico e fase de execução.

**Estados:** Executando

**Entradas:** Carta/Concluir

**Saídas:** Avaliação

**Regras:** C:interação antes nota

**Dependências:** Presenter

**Visual:** Formas animadas e botão Concluir exercício.

**Erros:** Duplicação rejeitada

**Aceitação:** AC11

### S12 Resultado

**Responsabilidade:** Aplicação única. Objetivo: Registrar.

**Dados:** Nota, preferência, pontos acumulados e quantidade de resultados.

**Estados:** Avaliação/resultado

**Entradas:** Fim

**Saídas:** Nota/pontos

**Regras:** C:antes Continuar

**Dependências:** Regras

**Visual:** Painel com cliente, nota e pontos; botão Continuar.

**Erros:** Não reaplicar

**Aceitação:** AC12

### S13 Saída

**Responsabilidade:** Cleanup. Objetivo: Encerrar.

**Dados:** Fase de saída e resultado já registrado.

**Estados:** Saída/fim

**Entradas:** Continuar

**Saídas:** Fim fatia

**Regras:** C:após resultado;A:fim fatia

**Dependências:** Presenter

**Visual:** Ator sai em0,8s; tela de atendimento concluído.

**Erros:** Fase obsoleta rejeitada

**Aceitação:** AC13

### S14 Recompra

**Responsabilidade:** Carga. Objetivo: Futuro.

**Dados:** Cargas atuais; inicial/reset ainda UNKNOWN.

**Estados:** Disponível/não

**Entradas:** Clique/carga

**Saídas:** Mão

**Regras:** C:decremento;U:inicial/reset

**Dependências:** Deck

**Visual:** Futuro botão indisponível com zero cargas; ausente agora.

**Erros:** Fora desta etapa

**Aceitação:** AC14 futuro

### S15 Dias

**Responsabilidade:** Cota/KO. Objetivo: Futuro.

**Dados:** Dias3/4/5/6/6, roster, KO e extraTurns FIFO.

**Estados:** 5dias referência

**Entradas:** Clientes/turnos

**Saídas:** Dia encerrado

**Regras:** C:Detention

**Dependências:** Roster/stamina

**Visual:** Futuras telas de fim e aviso; ausentes agora.

**Erros:** Não inventar dia1cliente

**Aceitação:** AC15 futuro

### S16 Progresso

**Responsabilidade:** Soma/clamp. Objetivo: Futuro.

**Dados:** Scores,T,M500,P35,E0 e saldo de pontos de presentes.

**Estados:** Intermediário/final

**Entradas:** Scores/T/P/E

**Saídas:** Progresso/saldo

**Regras:** C:S/Tnovo/S+P*E

**Dependências:** Dias

**Visual:** Futuros progresso/resultado/melhorias; ausentes agora.

**Erros:** U:medalhas

**Aceitação:** AC16 futuro

### S17 Eventos

**Responsabilidade:** Filas/flags. Objetivo: Futuro.

**Dados:** Filas0/1/2, flags de importância/interrupção, evento atual.

**Estados:** General/SkillEnded/EndDay

**Entradas:** Checkpoints

**Saídas:** Apresentação

**Regras:** C:players distintos

**Dependências:** Turno

**Visual:** Futura apresentação por player; nenhum evento sintético agora.

**Erros:** U:produtores completos

**Aceitação:** AC17 futuro

### S18 Save

**Responsabilidade:** Persistência própria. Objetivo: Futuro.

**Dados:** Esquema próprio futuro; não definido nem gravado nesta fatia.

**Estados:** Não implementado

**Entradas:** Estado

**Saídas:** DTO futuro

**Regras:** U:regras completas

**Dependências:** Modelo estável

**Visual:** Sem botões de save/load funcionais nesta etapa.

**Erros:** Sem escrita agora

**Aceitação:** AC18 futuro

### S19 Galeria/loja

**Responsabilidade:** Navegação. Objetivo: Futuro.

**Dados:** Catálogo visual próprio futuro; economia completa UNKNOWN.

**Estados:** Não implementado

**Entradas:** Catálogo

**Saídas:** Replay/compra

**Regras:** V:telas;U:economia

**Dependências:** Progresso

**Visual:** Sem galeria/loja nesta etapa.

**Erros:** Não alegar funcional

**Aceitação:** AC19 futuro

## Aceitação da primeira fatia — regressão histórica

- AC01: cena própria abre, Iniciar mostra2ou3 clientes, sem dependência da instalação original.
- AC02–04: escolher remove cliente disponível; entrada precede3cartas; segunda seleção bloqueada.
- AC05–06: normal3/clamp0..5, reciclagem/mão curta; prévia não cobra nem pontua.
- AC07: confirmação descarta toda mão e cobra uma vez; arraste baixo retorna, alto confirma pelo centro.
- AC08: Likes/Dislikes isolados t1/Likes; t20/21; preferência de habilidade acumula com tags.
- AC09:4raridades/6notas/tabela exata; Favorite independe da raridade.
- AC10: custos/floor float32/clamp; custo maior que saldo não cancela visita; zero sem revive/infinito inalterados.
- AC11–12: atividade precede avaliação; score antes de Continuar e uma vez.
- AC13: saída termina fatia, sem campanha/dia/loja/save; reabrir cria estado novo em memória.

Testes de domínio não comprovam cliques. Integração usa Mouse virtual/InputSystem/EventSystem e controles renderizados ou roteiro manual se bloqueado. Registrar execução e limites em FIRST_PLAYABLE_VALIDATION.md.

## Aceitação atual — fase 5

- FD01: menu com Nova sessão, Créditos/retorno e Sair funcionais no player, sem opções fictícias.
- FD02: preparar dia1 com cota3, filtro de elegibilidade e complemento documentados; não executar dias futuros.
- FD03: três visitas compartilham deck, stamina e pontos; descarte integral e reciclagem, sem reset por cliente.
- FD04: seleção remove cliente disponível; histórico conserva resultados, sem repetição indevida ou crédito duplicado.
- FD05: habilidade que zera stamina conclui atividade, resultado e saída; só então encerra dia, com precedência local sobre roster esgotado.
- FD06: roster esgotado encerra dia; último cliente pode aparecer como uma opção, sem inventar reposição.
- FD07: resultado diário mostra cada cliente/nota/pontos, soma, acumulado, progresso e motivo; sem recompensas ou economia inventada.
- FD08: chegada antes da fala, fala antes da mão; reação antes do próximo roster; treinadora e cliente visíveis na sala própria.
- FD09: retorno ao menu não duplica hierarquia; nova sessão tem dados novos, stamina100 provisória e pontos0.
- FD10: preservar os28 testes anteriores, executar novos EditMode/PlayMode e distinguir cliques reais Windows de renders/testes no Editor. Documentar limites de drag nativo e fechamento por atalho.
