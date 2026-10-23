# Arquitetura independente — v0.2

## Integração da fase 5

Produção passa a usar `FirstDay.unity`, com `FirstVisitView` configurada em modo diário. `FirstVisit.unity` e os 28 testes anteriores permanecem como regressão. Não há outro projeto Unity nem outro calculador de regras.

`GameplayDay` possui um `ClientRoster`, um `ExerciseDeck` e um `VisitResources` compartilhados. Cada nova `FirstVisit` recebe referências a esse deck/ledger, em vez de reiniciá-los. `VisitRecord` imutável é criado ao aplicar a avaliação; a UI lê os pontos registrados. Ao completar saída, o coordenador decide KO local, esgotamento ou próxima visita. `DayResult` é um snapshot criado uma vez com histórico, soma, acumulado, progresso saturado e pontos de presentes ainda não convertidos.

`StageProfile` conserva as cinco cotas de referência como dados; o coordenador implementa somente dia1. `RosterEntry.Level` é dado de elegibilidade, separado do histórico: sem inventar fórmula de LevelUp. Roster prioriza nível>0 e complementa sem repetir, conforme evidência/inferência documentadas.

O modo diário reutiliza a hierarquia e o presenter da visita. Menu, créditos e resultado diário são painéis contextuais da mesma raiz, sem carregar cenas a cada cliente. Retorno ao menu limpa referências da sessão; nova sessão cria um novo coordenador. Nenhuma persistência, recompensa ou evento sintético foi introduzido. As observações abaixo descrevem a arquitetura anterior quando se referem à fatia única.

23/10/2026. Substitui contratos v0.1 incompatíveis com fidelidade. Autorização atual: somente primeiro atendimento. GAMEPLAY_SPEC e FIDELITY_MATRIX são normativos.

| Área | Responsabilidade |
|---|---|
| Domain | C# sem UnityEngine: dados imutáveis, regras de preferência/nota/custo, deck, estado da visita; único dono das mutações |
| Runtime | Fixture própria, coordenador de comandos, uGUI/InputSystem e presenter de entrada/atividade/saída |
| Editor | Criar cena dedicada/build própria, sem reescrever SampleScene nem importar pesquisa |
| Tests/EditMode | Casos/fronteiras recuperadas, conservação do deck, transições, custo e score únicos |
| Tests/PlayMode | Mouse virtual/InputSystem e raycasting real; botões/arraste, screenshot do jogo próprio |

Referências diretas, sem DI ou bus global. Domain independente; Runtime depende Domain, uGUI, InputSystem, TMP já disponíveis. Coroutines pertencem à view e param ao desativar. Fórmulas ficam no domínio. Confirmação guarda referência, descarta toda mão, cobra e muda fase. Conclusão credita antes de Continuar. Saída não avança dia nem cria save.

Cena dedicada Assets/ProblematicExercise/Scenes/FirstVisit.unity com bootstrap explícito, gerada pelo Editor. Canvas Overlay1280×720, GraphicRaycaster, um EventSystem/InputSystemUIInputModule. TMP com recurso fornecido pelo pacote Unity; visuais de geometria/cores próprias. Sem dependência runtime de vídeos/Research/original.

RNG e max stamina100 são adaptações de fixture. Sem fallback de emergência, peso negativo para Dislikes ou bloqueio inventado de custo. Não implementar save/economia/eventos nesta etapa. Não usar PlayerPrefs. Lista explícita da cena na build, sem alterar BuildSettings global. Preservar GUIDs e cenas preexistentes; outputs de testes/build privados fora de Assets.
