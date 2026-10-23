# Evidências e método de análise nativa

## Atualização de status — fase 3

Novas evidências dirigidas, revisão de bytes, amostras visuais e decodificação local estão em StaticPhase3 e documentos 16–19. As fontes N1–N9 abaixo permanecem a evidência histórica da fase 2.

## Registro anterior preservado

23/10/2026. Esta etapa foi estática: arquivos PE/metadados foram lidos como dados. Não houve execução de GameAssembly, jogo, BepInEx, VisualProbe ou testes A/B/C. Nenhuma ferramenta nova foi instalada. Compatibilidade aqui significa capacidade de ler a build analisada, não compatibilidade funcional de modding.

## Fontes e ferramentas utilizadas

Raiz privada de pesquisa: `C:\Users\Micro\Documents\GitHub\GameGymPjct\ProblematicSubjects_Research`. Os caminhos desta tabela são relativos a essa raiz, não ao projeto Unity autoral.

| ID | Artefato / fonte | Função |
|---|---|---|
| E1–E5 | StaticPhase1 e Evidence, descritos no documento 01 | Contratos, configurações e inventários anteriores, reutilizados |
| N1 | WorkingCopy/GameAssembly.dll e global-metadata.dat na subpasta il2cpp_data/Metadata | Entradas nativas e metadados originais, somente leitura |
| N2 | Experiments/LoaderCopy/BepInEx/interop/MethodAddressToToken.db | Cache de endereços já existente; consultado sem executar o loader |
| N3 | StaticPhase2/tokens.json e native_map.json | Mono.Cecil: nomes/tokens dos wrappers e associação ao cache |
| N4 | StaticPhase2/original_metadata.json, extra_metadata.json, chain_metadata.json | LibCpp2IL: tipos originais, índices/tokens, RVAs, campos e offsets |
| N5 | StaticPhase2/pe_layout.json, gameplay_native.json | Cabeçalhos PE, seções, limites candidatos e instruções decodificadas com Iced |
| N6 | StaticPhase2/method_address_index.json e branch_witnesses.json | Correspondências verificadas, bytes de branches, tabela de saltos e constantes |
| N7 | StaticPhase2/integrity_check.json | Comparação SHA-256 dos 47 arquivos da WorkingCopy com baseline anterior |
| N8 | StaticPhase2/video_metadata.json | Somente contêineres MP4; não contém eventos observados ou frames |
| N9 | StaticPhase2/sum_native.json, round_native.json e numeric_witnesses.json | Auxiliares seguidos na revisão final: acumulação diária e floor do delta de stamina |

**CONFIRMED:** LibCpp2IL existente aceitou metadados 39 e identificou registros de código/metadados nesta build. Mono.Cecil leu assemblies de interop. Iced decodificou x64 dos intervalos selecionados. Foram usados Python local e o SDK .NET já instalado com Unity para compilar helpers de análise próprios. Não foi necessário usar AssetRipper para reconstruir cenas nem executar AssetStudio ou um mod loader.

Scripts originais de análise em StaticPhase2: `map_native.py`, `Tokens.cs`, `Meta.cs`, `Disasm.cs`, `render_native.py`, `calls.py`, `audit.py`, `video_metadata.py`; `build.py` compila apenas os helpers. Dependências foram reutilizadas da cópia histórica do loader; seus plugins não foram carregados. Logs e leituras anotadas permanecem fora do repositório autoral.

## Como os identificadores foram separados

**CONFIRMED:** base preferencial do PE = `0x180000000`. VA estática = base + RVA. Offset de arquivo foi calculado pela seção PE, e não pela subtração da base. ASLR poderia mudar a base em execução, mas não houve processo para observar.

O cache N2 foi associado aos tokens dos wrappers de N3; em seguida os métodos prioritários foram conferidos com N4, obtido dos arquivos originais. Nomes de máquinas assíncronas foram normalizados, por exemplo `StandardStageTurn+<PlayNextStep>d__6` nos originais e `StandardStageTurn/_PlayNextStep_d__6` nos wrappers. O token do wrapper não foi usado como se fosse token original.

Tabela representativa; números hexadecimais nas colunas de endereço/token. O índice é a posição original, zero-based, em `methodDefs`, não RVA nem número da linha do documento.

| Corpo efetivamente examinado | Índice original | Token original | Token wrapper | RVA | VA | Offset de arquivo |
|---|---:|---|---|---|---|---|
| ScoreCalculator.Calculate | 41181 | 06000b7b | 06001750 | 18abe10 | 1818abe10 | 18aa810 |
| ClientCharacter.InternalCalculatePreference | 41191 | 06000b85 | 0600175c | 18ac0e0 | 1818ac0e0 | 18aaae0 |
| SkillPlayer.PlaySkill/MoveNext | 40959 | 06000a9d | 06003bd6 | 18bb880 | 1818bb880 | 18ba280 |
| StandardStageTurn.ChooseClient/MoveNext | 41704 | 06000d86 | 06003e84 | 18d08e0 | 1818d08e0 | 18cf2e0 |
| StandardStageTurn.InternalPlayStep/MoveNext | 41706 | 06000d88 | 06003e97 | 18d1850 | 1818d1850 | 18d0250 |
| StandardStageTurn.MainClientPhase/MoveNext | 41708 | 06000d8a | 06003ea8 | 18d2d50 | 1818d2d50 | 18d1750 |
| ClientVisitHelper.MakeClientVisit/MoveNext | 41644 | 06000d4a | 06003e3d | 18d2fd0 | 1818d2fd0 | 18d19d0 |
| StageDayPlayer.PlayDay/MoveNext | 41640 | 06000d46 | 06003e26 | 18d3f50 | 1818d3f50 | 18d2950 |
| StandardStageTurn.PlayNextStep/MoveNext | 41710 | 06000d8c | 06003eb9 | 18d60b0 | 1818d60b0 | 18d4ab0 |
| StageTurnPlayer.Play/MoveNext | 41670 | 06000d64 | 06003e62 | 18d81f0 | 1818d81f0 | 18d6bf0 |
| PlayerScore.GradeValue | 42542 | 060010cc | 06002243 | 18e3830 | 1818e3830 | 18e2230 |
| Player.CalculateEndOfRunResult | 42513 | 060010af | 060021fb | 18e53a0 | 1818e53a0 | 18e3da0 |
| SkillMenu.WaitForSkillChoice/MoveNext | 43055 | 060012cd | 060043af | 18f66d0 | 1818f66d0 | 18f50d0 |
| EndOfDayResult.CalculatePresentsGainedInPoints | 44018 | 06001690 | 06002e2f | 19158a0 | 1819158a0 | 19142a0 |
| EndOfDayResult.CalculateTrueEndingProgress | 44019 | 06001691 | 06002e30 | 19159e0 | 1819159e0 | 19143e0 |

Assinaturas de fronteira conferidas: `PlaySkill(ActiveSkill, ClientCharacter, CancellationToken)`, `Calculate(ClientCharacter, ActiveSkill) → PlayerScore`, `WaitForSkillChoice(CancellationToken) → UniTask<ActiveSkill>`, `CalculatePresentsGainedInPoints(int gradePointsPerPresent, int extraFreePresents) → int`. O detalhe exato dos tipos consta de N3/N4; o corpo operacional de um async é MoveNext, não o wrapper que cria a tarefa.

## Evidências de fluxo e cálculo

| Descoberta | Evidência discriminante | Grau / escopo |
|---|---|---|
| Avanço do turno | Incremento de currentStep antes de InternalPlayStep em `18d61a5`; escrita Done em `18d6167` | **CONFIRMED**, caminho normal |
| Resultados de etapa diferentes | Campos flag/atraso/callback nos retornos de `18d1850`; callback em `18cf840` alcança EndComing | **CONFIRMED** |
| Uma visita no turno padrão | MainClientPhase delega a MakeClientVisit; grafo normal deste contém uma escolha e uma execução | **CONFIRMED** no helper padrão; não generaliza extensões |
| Efeito da flag de continuação | Teste do byte em `18d85f0`; verdadeiro volta ao loop, falso segue para IsKO | **CONFIRMED** |
| Confirmação por arraste | Comparação de Y com altura/3, `1913813–191382e`; evento OnChoose condicionado ao resultado | **CONFIRMED**, caminho de ponteiro |
| Compra normal 3 e teto 5 | Construtor `18e0705`; clamp em PrepareHand e DrawActiveSkills | **CONFIRMED**; compra explícita tem outro caminho |
| Likes e Dislikes convergem | Dois branches alcançam `18ac3c6`, que incrementa o mesmo acumulador | **CONFIRMED** no binário, intenção **UNKNOWN** |
| Favorite e classificação | Soma 25 em `18ac3ba`; comparação estrita com 20 em `18ac410` e seguintes | **CONFIRMED** |
| Raridade/preferência viram nota | Branch de Favorite, quatro bases por raridade e ajustes em `18abea3–18abf17` | **CONFIRMED** |
| Enum não equivale a pontos | Tabela de seis destinos em `18e3920`, retornos 10/14/18/22/26/34 | **CONFIRMED** |
| Redução diária | Seed zero; laço em `6ee621–6ee63b` passa acumulador/elemento; delegate `19279b7` soma GradeValue ao acumulador | **CONFIRMED** como soma no domínio int32 sem overflow |
| Arredondamento de stamina | Auxiliar `3d3868–3d3917` trunca fração e subtrai 1 para negativo não inteiro; constantes -1/+1 verificadas | **CONFIRMED** como floor para finitos normais |
| Aplicação de progresso | Soma e saturação em `18e5353–18e537a` | **CONFIRMED** |
| Final | Override ou comparação progresso/máximo em `18e53dc–18e5433` | **CONFIRMED** |
| Último dia | Teste `Day+1c` após PlayDay em `1909371` | **CONFIRMED** |

Outros corpos examinados por finalidade: SkillHand.FindSkill e predicado; SkillMenuLogic.PrepareHand/DiscardAndRedraw; ActiveSkillDeck.DrawActiveSkills/InternalDrawActiveSkills/DiscardDrawnSkills/IncreaseHandSize; RerollStageAddon.HandleClick e UIRerollButton.SetUp; ClientCharacterRoster.SetUp/HasClients/ShowAndAWaitResponse; ClientCharacterRosterLogic.ObtainClientsForDay/ObtainCharactersToShowInRound; StageTurnProvider.NextTurn; Player.AddNewScore, PlayerScores.AddNewScore e Player.DaySetUp; Stamina.Change/Decrease/ApplyCostReduction/IsOutOfStamina; SkillStaminaCostHelper e handlers comum/Gym; DayProvider.NextDay/construtor; EndOfDayResult/construtor; TrueEndingProgress; GameplayEventDirector.PlayQueuedEvents/MoveNext; StagePlayerScreen.LevelFlow/MoveNext. Nem todo auxiliar chamado foi resolvido.

## Limitações que mudaram a análise

**CONFIRMED:** entradas `.pdata` não delimitam necessariamente um método inteiro otimizado: há funções divididas em fragmentos. A primeira leitura curta de Calculate omitia branches posteriores. A seleção foi ampliada até o próximo RVA mapeado, preservando os fragmentos `.pdata`; esse intervalo é somente um limite candidato, pode incluir padding e tabelas.

**CONFIRMED:** decodificação linear de uma tabela de saltos produz instruções aparentes sem significado operacional. A tabela de GradeValue foi relida como seis inteiros RVAs. Branches e continuações foram seguidos para reconstruir caminhos; não se somaram todas as instruções decodificadas como se todas fossem executáveis.

**CONFIRMED:** funções compartilhadas e no-ops possuem muitos aliases, por exemplo `418b70`. O primeiro nome mostrado pelo renderizador pode ser totalmente alheio ao chamador. Não foi usado como prova de que a rotina de gameplay chamou aquele sistema. Chamadas genéricas/virtuais sem resolução suficiente continuam inferidas ou desconhecidas.

**UNKNOWN:** comportamento global de custos fora do domínio normal, totalidade dos efeitos de addons, semântica de alguns filtros de roster, todos os produtores de turnos extras e garantias de recuperação após cancelamento. A redução diária e o arredondamento foram resolvidos em N9 na revisão final. O nome original do auxiliar floor permanece desconhecido, sem impedir a identificação de suas operações. Não foi recuperado código-fonte C# original nem um projeto Unity proprietário. A análise é uma reconstrução funcional em linguagem própria, parcial e específica desta build.

## Preservação

**CONFIRMED por N7:** os 47 arquivos da WorkingCopy correspondem em tamanho e SHA-256 ao baseline `Evidence/workingcopy_before.json`, sem arquivos novos ou ausentes. GameAssembly SHA-256: `2c226039aaaecdd087db7876336fd10248bad1b1e88da714aace10fd8c8d293c`.

O ZIP em Original não foi aberto para escrita, recomposto ou usado como destino. Saves, registro, perfil PE_Tester, conta pessoal e Gym Discipline não foram usados. A comparação de hashes demonstra a integridade da WorkingCopy; não é apresentada como auditoria forense de todo o Windows. Artefatos nativos e dependências permanecem na pesquisa privada. No projeto autoral entraram apenas estes documentos, sem imports em Assets, commit ou push.
