# Elenco, eventos e revisão de preferências — fase 3

23/10/2026. Investigação estática dirigida, sem executar o jogo. Endereços são RVAs da build estudada. As fórmulas já recuperadas nos documentos 03 e 12 não foram reinvestigadas genericamente.

## Fontes desta fase

Raiz privada: `C:\Users\Micro\Documents\GitHub\GameGymPjct\ProblematicSubjects_Research\StaticPhase3`.

| Fonte | Conteúdo e alcance |
|---|---|
| P3-N | native.json, original_metadata.json; leituras *_native.txt | Corpos selecionados com o mapa da fase 2; metadados originais e continuações necessárias |
| P3-P | preference_review.json e preference_review.txt | Verificação independente dos saltos por bytes e revisão da agregação |
| P3-V | visual_sample.json e visual_links.json | Amostra serializada e ligações de componentes; documento 17 |
| P3-M | video_metadata.json, video_*_log.txt, Captures | Metadados dos MP4 e quadros de pesquisa; documento 18 |

As ferramentas existentes Mono.Cecil, LibCpp2IL, Iced e UnityPy foram reutilizadas. Helpers próprios ficaram na pesquisa privada. Intervalos decodificados não equivalem a métodos inteiramente compreendidos; chamadas compartilhadas sem identificação suficiente continuam inferidas.

## Seleção diária e filtros

**CONFIRMED:** a interface original `IReadOnlyRosterData` tem, nessa ordem, AllClients, AvailableClients, ClientBy e HasRandomClientsEveryday. Isso permite identificar as chamadas de interface do algoritmo, em vez de deduzir seus argumentos pelos nomes do wrapper.

**CONFIRMED:** o predicado usado na seleção diária não é um teste de “não atendido”: `ClientCharacterRosterLogic.<ObtainClientsForDay>b__0_0`, RVA `18bf7a0`, compara o campo `ClientCharacter.currentLevel` em +0x58 com zero e retorna **level > 0**. Campo e offset foram conferidos nos metadados originais da fase 2. Clientes novos e clientes já evoluídos recebem tratamento diferente nesse filtro.

**STRONGLY INFERRED — algoritmo de `ObtainClientsForDay`, `18ab1b0`:**

1. Se HasRandomClientsEveryday é true, obtém AllClients, embaralha e toma até a quantidade diária.
2. Caso contrário, obtém AllClients, aplica o filtro level>0, embaralha e toma até a quantidade diária.
3. Se faltam clientes, completa a seleção usando o restante de AllClients, com Except para excluir os já selecionados, outro embaralhamento e a quantidade que falta.
4. O roster substitui sua lista disponível pelo resultado e a embaralha (`18ab930`).

A flag, o predicado, os branches e a chamada Except são **CONFIRMED**. Algumas operações genéricas da sequência de filtragem/Take/materialização não possuem nome inequívoco no cache; o algoritmo composto mantém a classificação acima. Não se afirma distribuição uniforme ou probabilidade exata.

**CONFIRMED:** a rodada padrão chama `Random.Range(2,4)` (`18ab171`); o fluxo ao redor é compatível com apresentar até duas ou três opções dos disponíveis. **STRONGLY INFERRED:** a mão de clientes é uma amostra limitada, não a quantidade inteira do dia. A busca por ID e remoção do escolhido da lista disponível foram recuperadas na fase 2. Não há garantia de quantidade exata quando o catálogo é insuficiente.

## Cliente obrigatório e lógica substituída

**CONFIRMED:** `DetentionTutorialClientCharacterRosterLogic.ObtainCharactersToShowInRound`, `18fcf80`, cria um array de uma posição e resolve `ClientBy(1)`. A implementação diária chama a base, testa a presença desse cliente e, se ausente, substitui a primeira posição por ele (`18fd050`, especialmente `18fd17d–18fd1cf`; predicado auxiliar em `190d360`). Logo existe uma seleção obrigatória de tutorial, diferente da seleção padrão.

**UNKNOWN:** comportamento para catálogo sem esse ID ou quantidade diária zero; os branches vistos têm caminhos de erro, não um fallback autoral demonstrado. O ID original serve apenas como evidência e não será um ID de produção do novo jogo.

**CONFIRMED:** o roster contém lógica base e override. **UNKNOWN:** lista completa de todos os produtores de overrides. A existência do tutorial não autoriza concluir que todo dia tenha cliente obrigatório.

## Turnos extras e visitas fora do turno

**CONFIRMED:** StageTurnProvider testa sua lista extraTurns, remove a primeira posição quando há extras e só constrói StandardStageTurn quando a lista está vazia (`18cef90`). É possível o provedor continuar sem clientes enquanto houver extras; ausência de ambos o esgota. DayCleanUp limpa essa lista.

**UNKNOWN após busca dirigida:** não foi identificado um produtor concreto que acrescente um IStageTurn nessa lista. O contrato do provedor não expõe uma operação pública de inserção, e o inventário anterior mostra StandardStageTurn como implementação evidente de PlayNextStep. StageCommandDependencies mantém uma referência ao provedor, mas isso não demonstra uso. A infraestrutura não prova que algum evento observado insira turno extra. Não se declara a lista “inutilizada” em toda a build.

**CONFIRMED — exceção relevante e distinta:** `SecretSkillGameplayEventPlayer.<Play>d__2.MoveNext`, `18d70c0`, esconde o roster, cria habilidade e cliente definidos pelo evento, instala `SecretSkillMenuLogic` (`18d73c2`) e chama **ClientVisitHelper.MakeClientVisit diretamente** (`18d744f`). Na continuação normal limpa o override (`18d7181`), verifica KO e volta a exibir o roster quando cabível. Isso produz uma visita pelo caminho de evento sem passar pela seleção convencional de StandardStageTurn; não depende de demonstrar inserção em extraTurns.

**CONFIRMED por contrato e corpo:** SecretSkillMenuLogic recebe uma habilidade alvo e seu DiscardAndRedraw é uma implementação vazia compartilhada; não se deve pressupor o mesmo reroll da mão normal. **UNKNOWN:** todos os gatilhos e invariantes de limpeza desse evento em cancelamento.

## Eventos narrativos e interrupções

**CONFIRMED:** `GameplayEventDirector.InternalEnqueueEvent`, `18c6a30`, usa a estrutura de filas por fase e o caminho de notificação de evento enfileirado. As filas General, SkillPlayEnded e EndOfDay são processadas em pontos distintos já localizados. `ScheduleProgressionEventCommand.Play`, `18b2270`, obtém um evento de progressão, constrói um registro agendado e chama Schedule. Enfileirar agora e agendar são operações separadas.

**CONFIRMED:** a espera de cliente registra um callback do diretor; o diretor mantém token vinculado de espera. Em `18c6b5e–18c6b75`, o cancelamento depende de **jumpsAndInterrupts=true e inWait=true**. Os metadados originais confirmam jumpsAndInterrupts em +0x1c e isImportant em +0x1d: são flags distintas. Não é correto atribuir a interrupção somente à importância do evento. Quando jumpsAndInterrupts está ativo e a fila já tem itens, existe ainda caminho de inserção prioritária; a identificação da operação genérica como inserção na primeira posição é **STRONGLY INFERRED**.

A fase 2 mostrou verificação de KO, descarte de eventos não importantes e retorno de KO pelo processamento das filas. Não generalizar isso como cancelamento destrutivo de qualquer atendimento já iniciado. Fonte adicional dos offsets: event_config_metadata.json.

**CONFIRMED:** `VisitingCharacterDialogEventPlayer.<Play>d__3.MoveNext`, `18d7ae0`, muda o WorldStage, coloca visitante, constrói diálogo de mundo, faz transições de saída e reseta visitantes (`18d80ee`). Não apareceu nesse corpo o caminho padrão WaitForSkillChoice → PlaySkill → Calculate. Um visitante narrativo não é necessariamente um cliente avaliável.

**OBSERVED nos vídeos:** avisos de mensagem e um visitante depois da transição para o dia seguinte aparecem em 04; associação exata de cada um a uma configuração do diretor é **INFERRED**. Não foram transcritos diálogos nem catalogados todos os eventos.

## Revisão independente da anomalia Likes/Dislikes

**CONFIRMED:** enum original tem Likes=0, Dislikes=1, Ambivalent=2, Favorite=3. A rotina lê um inteiro de 32 bits da configuração da preferência; não apareceu conversão de sinal, float, normalização de peso ou reinterpret_cast entre essa leitura e o branch examinado.

Além do Iced, um leitor independente decodificou diretamente dois saltos curtos x86 do arquivo:

| RVA do salto | Bytes | Deslocamento assinado | Destino calculado |
|---|---|---:|---|
| 18ac3a5 | 74 1f | +31 | 18ac3c6 |
| 18ac3aa | 74 1a | +26 | 18ac3c6 |

**CONFIRMED:** ambos alcançam bytes `ff c7`, incremento de EDI. O primeiro branch é após testar preferência zero; o segundo, após subtrair 1. Ambivalent pula a contribuição; Favorite soma 25. A classificação final usa comparações assinadas: positivo até 20 é Likes, acima de 20 Favorite, zero Ambivalent e negativo Dislikes. O resultado intermediário não é invertido antes da classificação.

**CONFIRMED no escopo estático:** a interpretação anterior da convergência permanece sustentada por bytes e metadados, não somente pelo formatador. Otimização pode compartilhar um destino comum; não há evidência de que essa otimização esconda uma subtração diferente para Dislikes nesse caminho.

**UNKNOWN:** intenção dos autores e comportamento de todos os dados em runtime. A tentativa adicional de localizar preferências inline nas bibliotecas selecionadas não produziu amostras adequadas; isso está explícito no JSON, e não foi tratado como confirmação serializada. Vídeo de uma nota isolada também não identifica todos os inputs necessários para provar a fórmula.

**Decisão vigente — correção de fidelidade do usuário:** preservar Likes e Dislikes contribuindo igualmente +1, com os limites recuperados. A recomendação anterior de pesos negativos e testes de monotonicidade foi **revogada**, não implementada. A possibilidade de evitar penalizações inevitáveis para clientes difíceis é uma hipótese de design; não comprova intenção dos autores. Consultar GAMEPLAY_SPEC v0.2 e FIDELITY_MATRIX.

## Encerramento da investigação deste bloco

Filtros de nível, cliente obrigatório de tutorial e visita direta por evento explicam diferenças relevantes. Permanecem desconhecidos o produtor efetivo de extraTurns e o catálogo integral de gatilhos/addons. Não é necessário resolver todas essas extensões para implementar o escopo autoral delimitado em GAMEPLAY_SPEC.
