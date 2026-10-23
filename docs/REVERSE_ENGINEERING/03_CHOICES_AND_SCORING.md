# Escolhas, habilidades e pontuação — etapa 2

## Correção de fidelidade — 23/10/2026

As propostas autorais de rebalanceamento da especificação v0.1 foram revogadas. A regra recuperada Likes/Dislikes +1 será preservada. A implementação de uma visita foi autorizada pela instrução posterior do usuário. Consultar docs/DESIGN/GAMEPLAY_SPEC.md v0.2 e FIDELITY_MATRIX.md; afirmações históricas abaixo sobre pesos negativos, três dias ou espera de aprovação não são diretrizes vigentes.

## Atualização de status — fase 3

A revisão independente da fase 3 confirmou os dois saltos brutos para o incremento comum Likes/Dislikes; intenção e validação dinâmica do original continuam desconhecidas. Ver 16_ROSTER_AND_EVENTS. Após a correção do usuário, a especificação independente v0.2 preserva essa convergência; a antiga proposta de pesos negativos foi revogada.

## Registro anterior preservado

23/10/2026. Investigação estática; nenhuma partida foi executada. As classificações descrevem a evidência disponível nesta build, não a intenção dos autores. Endereços abaixo são **RVAs**, salvo indicação explícita. Fontes, correspondências com metadados e limitações: [13_NATIVE_ANALYSIS.md](<C:/Users/Micro/Documents/GitHub/GameGymPjct/Problematic Exercise/docs/REVERSE_ENGINEERING/13_NATIVE_ANALYSIS.md>).

## Deck e mão

**CONFIRMED — metadados originais e corpos nativos:** `ActiveSkillDeck` mantém listas distintas de todas as habilidades, compra, descarte e habilidades compradas, além de tags requeridas e tamanho da mão. Seu construtor escreve `handSize=3` (`18e0705`); o literal original `DefaultHandSize` também vale 3.

| Operação | Comportamento recuperado | Evidência / confiança |
|---|---|---|
| PrepareHand | Lê o tamanho do deck, limita a 0–5, compra, acrescenta à SkillHand e envia dados de apresentação à UI | `SkillMenuLogic.PrepareHand`, `18e91e0`; **CONFIRMED** |
| DrawActiveSkills | O argumento padrão -1 usa tamanho limitado a 0–5. Quantidade explícita segue outro caminho, sem esse clamp no método | `18dfb10`; **CONFIRMED**. Cinco não é limite universal de toda compra adicional |
| IncreaseHandSize | Soma o incremento ao campo; a limitação ocorre na compra normal | `18dfc90`; **CONFIRMED** |
| Descarte | Acrescenta **todas** as habilidades compradas ao descarte e limpa o registro de compras | `18dfa80`; **CONFIRMED** |
| Reposição | Quando a compra fica vazia, transfere descarte para compra e embaralha. Se ambas estão vazias, pode devolver menos opções que o solicitado | `18dfca0`, especialmente `18dfe44–18dff3c`; **CONFIRMED** nos branches e listas; identificação de algumas operações genéricas **STRONGLY INFERRED** |
| Tags requeridas | A compra processa tags antes de completar a quantidade; tenta a pilha de compra e depois o descarte | `18dfb40`, `18dfca0`; **CONFIRMED** na ordem de chamadas. Desempate e distribuição completa **UNKNOWN** |
| Recomprar | Limpa a mão, descarta as compras anteriores e compra outra mão | `SkillMenuLogic.DiscardAndRedraw`, `18e9060`; **CONFIRMED** |

**CONFIRMED:** reroll tem um recurso próprio. `RerollStageAddon.HandleClick` decrementa cargas antes de atualizar o botão e chamar a recompra (`18b1786–18b17b2`). `UIRerollButton.SetUp` considera disponível e ativa sua UI somente se cargas > 0 (`190ff4b–190ff6c`). O handler sozinho não verifica novamente esse limite. Não se conclui que reroll seja gratuito ou ilimitado porque `DiscardAndRedraw` não cobra stamina.

**CONFIRMED:** `IncreaseMaxRerollsPassiveEffect` alcança `SetCharges`; `IncreaseHandSize` modifica o tamanho. **UNKNOWN:** combinação de todos os passivos, recarga exata das cargas em cada estágio e catálogo completo de upgrades. `EnableSkills`/`DisableSkills` têm caminhos para devolver/remover habilidades do deck; isso não demonstra sozinho todas as regras de desbloqueio.

**UNKNOWN:** garantia global de IDs únicos e ausência de duplicatas, inclusive após overrides e compras adicionais; tratamento visual de mão vazia. O caminho de reposição pode retornar uma mão curta: não foi encontrada aqui uma regra universal de opção de emergência. Não foram recuperadas probabilidades de sorteio.

## Selecionar, arrastar e confirmar

**CONFIRMED:** os eventos `OnSelectSkillChanged`, `OnDragSkillChanged` e `OnChooseSkill` são distintos. Seleção e arraste fornecem dados para apresentação/prévia; a escolha fornece um inteiro para concluir a espera. O nome do parâmetro `instanceId` no wrapper não prova que exista uma identidade única por cópia de carta: a cadeia de UI consulta `SkillDisplayData.Id`, e a busca da mão compara o ID da configuração atual.

**CONFIRMED — caminho de ponteiro:** em `UISkill.HandlePointerDown` (`1913ae0`), o código testa o estado do arrastador e inicia o arraste, mudando parent e posição. Em `HandleOnPointerUp` (`1913760`), quando está arrastando e não está retornando, compara Y da posição da carta com `Screen.height / 3`. Se Y ≥ esse limiar, emite fim de arraste com `willBeChosen=true` e depois `OnChoose`; caso contrário, inicia retorno e não confirma. A constante 3 foi lida do PE; os campos de eventos foram conferidos nos metadados originais.

**UNKNOWN:** correspondência exata entre essas coordenadas, escala do Canvas e pixels em todas as resoluções; caminhos alternativos de controle. Não houve teste de mouse nesta fase. Selecionar para prévia não deve ser descrito como confirmação automática.

**CONFIRMED:** `UISkillMenu.HandleChooseSkill` marca a carta escolhida, limpa seleção, desabilita interação nas opções e transmite seu ID (`1911d90–1911f0a`). `SkillMenu.HandleChooseSkill` conclui a fonte assíncrona (`18e96e0`). `WaitForSkillChoice.MoveNext` (`18f66d0`) cria essa fonte e espera; ao receber o inteiro, resolve a habilidade na mão (`18f6dba`). O predicado de `FindSkill` compara o ID (`18f5550`). Isso valida pertencimento por busca, mas não constitui uma checagem completa de custo ou uma política de segurança da UI.

**STRONGLY INFERRED:** a entrada da espera aciona `ISkillMenuLogic.Start` e `PrepareHand` pelas posições da interface. **CONFIRMED:** no caminho normal de conclusão, a mão é limpa e `Player.DiscardDrawnSkills` é chamado (`18f6903–18f692f`) antes de a espera devolver a habilidade escolhida. Portanto, o descarte não significa que a habilidade foi removida permanentemente do catálogo; a referência escolhida permanece disponível para execução. Não foi encontrada uma repetição de escolha na visita padrão.

## Execução da habilidade

**CONFIRMED — fluxo normal de `SkillPlayer.<PlaySkill>d__11.MoveNext`, `18bb880`:** recebe habilidade, cliente e token; sinaliza uso (`18bba97`), consulta `ActivityId` (`18bbab3`), passa ID, personagens da configuração do cliente e duração para `ActivityPlayer.PlayActivity` (`18bbb0d`). Depois espera entrada visual, diálogo da atividade e a interação de conclusão da atividade. Só então passa pela animação de conclusão e calcula a nota (`18bc3c4`).

**CONFIRMED:** prepara a apresentação do resultado, acrescenta o score ao jogador (`18bc457`), mostra o resultado, anima mudanças de estatísticas, aguarda o comando de continuar (`DetectNext`, `18bc953`) e envia conclusão da atividade (`18bca83`). Score registrado e clique de continuar são momentos distintos. Os `await`s foram seguidos pelas continuações; não é uma ordem deduzida somente das assinaturas.

**CONFIRMED:** a consulta de preferências usada na nota acontece no calculador; a seleção da atividade usa `ActiveSkill.ActivityId`, não a nota final. O cliente fornece seu conjunto de personagens. **UNKNOWN:** resolução completa de IDs em todos os prefabs/poses, condições de todas as atividades e mudanças introduzidas por addons.

**STRONGLY INFERRED:** o sinal de habilidade jogada alimenta os listeners de consumo. Há handlers nativos específicos de stamina; a operação de custo não está embutida no calculador de nota. Detalhes e escopo por estágio no documento 12.

## Agregação de preferências

**CONFIRMED:** `ClientCharacter` possui dicionários por ID de habilidade e por ID de tag. `InternalCalculatePreference` (`18ac0e0`) reúne a preferência específica da habilidade, se houver, e as preferências correspondentes às tags retornadas pela habilidade. Não foi encontrada nessa rotina uma prioridade exclusiva da preferência específica sobre as tags: as correspondências são reunidas.

Os metadados originais confirmam `Likes=0`, `Dislikes=1`, `Ambivalent=2`, `Favorite=3`. O código lê esse enum na configuração de cada preferência. Para entradas válidas e sem overflow, a regra observada é:

| Preferência correspondente | Contribuição para o acumulador t | Evidência |
|---|---:|---|
| Likes | +1 | branch para `18ac3c6` |
| Dislikes | **+1** | segundo branch também para `18ac3c6` |
| Ambivalent | 0 | salto para a próxima entrada |
| Favorite | +25 | `18ac3ba`, segue sem o incremento comum |

**CONFIRMED no corpo analisado:** `t>20 → Favorite`; `0<t≤20 → Likes`; `t=0 → Ambivalent`; `t<0 → Dislikes` (`18ac410` e continuações). Enums fora do domínio seguem exceção. O ramo negativo existe, mas não apareceu uma contribuição negativa nessa acumulação. Com uma quantidade normal de entradas válidas, as contribuições observadas não o atingem.

**UNKNOWN:** por que Dislikes converge com Likes; se isso é defeito, comportamento deliberado ou resultado de uma particularidade da build. Não foi feita validação dinâmica. Não substituir +1 por -1 para tornar o resultado “mais intuitivo”. As instruções e bytes da convergência estão preservados em `branch_witnesses.json`.

**CONFIRMED:** `CheckPreferencesFor` (`18ac030`) chama a rotina e armazena `lastPreferencesResult`. `Calculate` também a chama e atualiza esse resultado. Não se trata de deduzir preferências por nomes ou por imagens.

## Nota e pontos

**CONFIRMED — `ClientCharacterScoreCalculator.Calculate`, `18abe10`:** para resultado de preferência p e raridade r∈{0,1,2,3}, a nota numérica g é:

```text
se p = Favorite: g = 5
caso contrário: g = (r + 1) + ajuste(p)
ajuste(Likes)=+1; ajuste(Dislikes)=-1; ajuste(Ambivalent)=0
```

**CONFIRMED:** g=0,1,2,3,4,5 representa D,C,B,A,S,W. A comparação de Favorite ocorre antes dos casos de raridade; os demais casos válidos de raridade produzem bases 1,2,3,4. Raridade não suportada nesse caminho segue exceção. Não há sorteio, arredondamento ou multiplicador de upgrade nesse cálculo local. Isso não exclui mudanças de raridade, tags ou configuração antes da chamada.

**CONFIRMED — `PlayerScore.GradeValue`, `18e3830`:** enum de nota e pontos são grandezas diferentes. A tabela de saltos foi lida como dados e conferida nos seis retornos:

| Nota | Enum g | Pontos |
|---|---:|---:|
| D | 0 | 10 |
| C | 1 | 14 |
| B | 2 | 18 |
| A | 3 | 22 |
| S | 4 | 26 |
| W | 5 | 34 |

**CONFIRMED:** resultado mantém ID do cliente, personagens, nota, resultado de preferências e raridade. `Player.AddNewScore` encaminha para `PlayerScores.AddNewScore`, que acrescenta o objeto à lista e emite atualização. O score não é apenas um texto transitório na tela.

## Exemplos sintéticos e limites

**CONFIRMED como consequências matemáticas das regras locais; não são partidas executadas:**

- Nenhuma correspondência: t=0, Ambivalent. Raridade 0 produz C, 14 pontos.
- Uma correspondência Likes: t=1, Likes. Raridade 0 produz B, 18 pontos.
- Uma correspondência Dislikes: o binário observado também produz t=1, Likes; com raridade 0, B e 18 pontos. Este exemplo evidencia a anomalia.
- Uma correspondência Favorite: t=25, Favorite; W e 34 pontos para qualquer raridade válida.
- Vinte correspondências de peso +1 ainda dão Likes; vinte e uma passam o limiar de Favorite. A existência desse número de tags/configurações em um cliente real não foi demonstrada.
- Se o calculador receber p=Dislikes e r=0, seu ramo retorna D/10. Isso demonstra o ramo do calculador, **não** demonstra que a agregação normal consiga produzir essa entrada.

**UNKNOWN:** impacto de todos os upgrades sobre entradas, duplicação de tags, pesos no material de apresentação versus agregação, exceções por estágio e mudanças futuras de build. A fórmula documenta o cálculo observado, não uma especificação a copiar para o jogo autoral.

**ORIGINAL DESIGN DECISION:** uma futura implementação independente deve definir seus próprios objetivos, preferências, notas, economia e parâmetros; especialmente deve decidir explicitamente como preferências negativas funcionam. Nenhuma implementação foi iniciada.
