# Problematic Exercise — handoff de desenvolvimento

## Identificação

- Projeto: Problematic Exercise.
- Diretório: `C:\Users\Micro\Documents\GitHub\GameGymPjct\Problematic Exercise`.
- Unity: **6000.6.6f1 (d20c9ac777da)**; não confundir com a versão do jogo investigado. Não atualizar pacotes para tentar corrigir sintomas.
- GitHub: https://github.com/nTales129/Problematic-Exercices-.git
- Branch de desenvolvimento: `phase6-campaign`.

## Objetivo e invariantes

Jogo independente de academia que busca alta fidelidade funcional ao gameplay investigado de Problematic Subjects. Todo código e conteúdo de produção deve ser próprio, com exceção de dependências Unity/TMP e fonte com suas licenças. Não importar código descompilado, sprites extraídos, binários, cenas ou conteúdo proprietário. Não retomar modding nem executar o original. Não modificar Gym Discipline, saves pessoais ou o ZIP original.

Regras normativas em `docs/DESIGN/GAMEPLAY_SPEC.md` e `FIDELITY_MATRIX.md`, adendo6 com precedência sobre limites históricos. Evidências de regras em `docs/REVERSE_ENGINEERING/03_CHOICES_AND_SCORING.md`, `11_TURN_STATE_MACHINE.md`, `12_DAY_AND_PROGRESSION.md`, `16_ROSTER_AND_EVENTS.md` e `18_REFERENCE_TIMELINE.md`. CONFIRMED é análise estática, OBSERVED é vídeo, não validação dinâmica do original. `docs/DEVELOPMENT/PHASE6_CAMPAIGN.md` contém as adaptações e evidências da entrega atual.

## Estado atual

Implementados: menu/créditos/Sair; cinco dias3/4/5/6/6; catálogo de seis adultos originais; roster prioritário/complementado sem repetição diária; seleção/entrada/fala/mão/prévia/confirmação/atividade/reação/resultado/saída; deck diário compartilhado com descarte integral e reciclagem; stamina e scores diários; sessão com histórico, saldos e estado por cliente; resultado diário e continuação direta; final após quinto dia; retorno ao menu e nova sessão limpa. UI uGUI e placeholders geométricos próprios, sala e treinadora persistentes.

Incompletos: arraste na build Windows ainda depende de evidência específica; crash histórico intermitente Alt+F4 sem causa confirmada; níveis sem fórmula LevelUp; contrato completo de reset/passivos desconhecido. Adaptações explícitas: stamina100/modificador1 e deck novo a cada dia, níveis fixos1/0, histórico de atendimentos separado, continuação sem seção de melhorias. Total continua acumulando acima500, progresso satura500.

Não iniciados/concluídos: recompra, requisitos completos de cartas, melhorias, tutorial, eventos/turnos extras, fontes especiais de KO, presentes/recompensas/medalhas, loja, galeria, save permanente, arte final e rigs complexos. Não criar botões falsos para esses sistemas.

Mapa de código em `Assets/ProblematicExercise`:

- Domain: VisitRules, ExerciseDeck, VisitResources, FirstVisit, GameplayDay e CampaignSession. Regras calculadas no domínio, não na UI.
- Runtime: FirstVisitView e parcial .Day; ExerciseCardView; VisitSample com fixtures/conteúdo próprio. ConfigureAsDay preserva regressões; ConfigureAsCampaign ativa campanha.
- Editor: CampaignTools prepara/abre/builda produção; FirstDayTools e FirstVisitTools preservam fatias antigas.
- Scenes: **Campaign.unity** produção; FirstDay.unity e FirstVisit.unity regressão.
- Tests/EditMode e Tests/PlayMode: regras, campanha e navegação por MouseState/EventSystem.

Como executar: abrir o projeto no Unity Hub com6000.6.6f1; abrir Campaign.unity ou menu Problematic Exercise → Open Campaign; Play. Nova sessão → cliente → aguardar chegada/fala → carta/Confirmar ou arraste → Concluir exercício → aguardar reação → Continuar. Próximo dia nos resultados intermediários, menu somente após final. Sem save: fechar descarta a sessão.

## Testes e Windows

49 testes aprovados:43 EditMode e6 PlayMode; PlayMode repetido após o ajuste tipográfico com6/6. Os39 anteriores foram preservados, sem editar seus quatro arquivos de testes. O novo PlayMode percorre os cinco dias por controles renderizados e reinicia uma sessão; não usa callbacks internos. Evidências privadas em `C:\Users\Micro\Documents\GitHub\GameGymPjct\ProblematicSubjects_Research\Phase6`. Build Succeeded/Errors=0. A auditoria confirmou WorkingCopy47/47 e vídeos5/5 intactos, Settings/ProjectSettings/Packages preservados e nenhum .meta ausente.

Nova execução Windows bloqueada por diálogo do Firewall não selecionável pela automação. Nenhuma permissão concedida ou clique de gameplay enviado nessa build; foi pedido ao usuário clicar Cancelar. A captura Windows/01-menu.png registra o bloqueio. Navegação/arraste/fechamento na nova build permanecem pendentes; não confundir com o percurso real da Fase5 nem com PlayMode. Retomar após dispensar manualmente esse diálogo, sem alterar segurança global.

A instância PID26804 foi encerrada por Stop-Process após verificação do caminho, somente para limpar o teste bloqueado; isso não valida shutdown normal. Registro privado blocked-test-cleanup.json. Reabrir a build para executar o roteiro manual; não há campanha a recuperar nem save persistente.

Build de desenvolvimento Windows64/Mono: `..\ProblematicSubjects_Research\Phase6\Build-Windows\ProblematicExercise.exe`, sempre com a pasta Data e DLLs acompanhantes. Não versionar binários. Preferir segundo monitor: `-screen-fullscreen 0 -screen-width 1280 -screen-height 720 -monitor 2`. Não afirmar navegação/drag do executável a partir de PlayMode.

Crash anterior: log Phase5/windows-final.log tem UnityPlayer/USER32/UIAutomationCore UiaDisconnectAllProviders durante shutdown, sem causa própria demonstrada. A build intacta encerrou por Alt+F4 nesta fase sem Crash quando não houve consulta UIA durante shutdown. Não declarar correção geral. Código próprio não mantém async/recursos nativos; o módulo de input faz seu próprio cleanup.

Computer Use disponível via skill computer-use e pacote @oai/sky em node_repl. Observar uma janela retornada, agir uma vez e recapturar; não consultar UIA durante fechamento. sky.drag não oferece duração: se não confirmar, usar roteiro manual no relatório, sem repetir tentativas indefinidamente. A mecânica de centroY≥altura/3 foi preservada; PE_CARD registra press/up/begin/release em desenvolvimento. Logs TLS60 e ComputeBuffer na saída também existiam antes; não atribuir causa automaticamente a drivers/Unity.

## Próxima etapa recomendada

Revisar a campanha entregue e concluir teste humano de drag baixo/alto e comparação de fechamento por Sair/Alt+F4 no menu e atividade. Não ampliar automaticamente o escopo. Depois, definir contrato autoral explícito para melhorias/recompra ou save, mantendo lacunas identificadas.

Próxima instrução exata para outra conversa: “Leia docs/HANDOFF.md e docs/DEVELOPMENT/PHASE6_CAMPAIGN.md; confira git status e os resultados XML da Phase6; retome somente a próxima prioridade registrada, sem executar Problematic Subjects nem alterar projetos vizinhos.”

## Git

O projeto não tinha Git próprio. Remoto inspecionado: somente commit inicial `f289a623c0487d9c671369a46f027241eb7defda` com gitignore. Repositório inicializado exclusivamente nesta pasta e branch phase6-campaign criada a partir de origin/main. Checkpoint da Fase5: `eb52dc9` (chore: checkpoint phase 5 independent Unity prototype). O histórico remoto foi preservado; nenhum reset, clean, sobrescrita ou force push.

Último commit de implementação da Fase6: **`4a68c22793ff1d63f8eb44df3cedd6da5f959e86`**, `feat: implement five-day campaign and update handoff`. **Push confirmado** na branch origin/phase6-campaign pelo mesmo hash em ls-remote. Este registro integra um commit documental posterior. O commit não pode incorporar seu próprio hash; HEAD exato, incluindo este registro, é obtido por `git rev-parse HEAD`, e a sincronização atual se verifica com `git ls-remote origin refs/heads/phase6-campaign`. `git log -2 --oneline` identifica implementação e atualização do handoff. Nenhum merge automático em main; desenvolvimento publicado em https://github.com/nTales129/Problematic-Exercices-/tree/phase6-campaign .

Excluídos: Library, Temp, Obj, Logs, UserSettings, builds, referências/GameplayVideos, pesquisa privada, executáveis/DLLs/interop do original, saves, vídeos, dumps, caches e arquivos locais do IDE. Assets próprios e seus .meta, Packages e ProjectSettings são versionados; TMP/Unity e licença da fonte preservados. Documentação de conclusões é própria; nenhum arquivo de pesquisa proprietário é incluído.

## Suggested Skills

- unity-workbench:unity-bug-investigation para estabilidade baseada em evidências.
- unity-workbench:unity-feature-implementation e unity-build-validation para mudanças autorizadas e verificação.
- computer-use:computer-use para testes reais na janela Windows; ler guidance/api antes de atuar.
- handoff para contexto conciso, respeitando o caminho docs/HANDOFF.md solicitado pelo usuário.
