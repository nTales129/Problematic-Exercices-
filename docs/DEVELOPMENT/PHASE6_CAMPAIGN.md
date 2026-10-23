# Fase 6 — campanha, estabilidade e checkpoint

23/10/2026. Produção independente em Unity 6000.6.6f1, sem atualização de Editor ou pacotes. As regras v0.2 continuam normativas. Este documento substitui os limites de escopo da Fase 5; o relatório anterior permanece como evidência histórica.

## Revisão de estabilidade anterior à expansão

O log privado `Phase5/windows-final.log`, linhas 494–497, registra shutdown do Input System seguido de `Crash!!!`. O stack nativo contém UnityPlayer sem símbolos, processamento de mensagens USER32, `UIAutomationCore.WindowPattern_WaitForInputIdle` e `UiaDisconnectAllProviders`. Não há stack gerenciado que identifique código do jogo como causa. A proximidade do shutdown do input não demonstra sua responsabilidade.

Inspeção própria: o runtime usa coroutines de entrada, reação e saída, interrompidas em OnDisable; não possui tarefas async, callbacks de shutdown, recursos nativos, Dispose ou OnDestroy próprios. O InputSystemUIInputModule instalado desabilita/desvincula suas ações em OnDisable e desfaz hooks em OnDestroy. Não foi acrescentado descarte duplicado, captura silenciosa de exceções ou atraso arbitrário de encerramento.

Antes de expandir o gameplay, a build intacta da Fase 5 foi aberta no monitor 2, menu observado e Alt+F4 enviado. Sem consulta UIA durante o shutdown, o PID 36680 terminou; `Phase6/baseline-altf4.log` chegou a CodeReloadManager destroyed e PlayerConnection Cleanup, sem Crash. Isso restringe a investigação, mas não prova que a consulta UIA causou o crash anterior nem resolve uma falha intermitente. O botão Sair já tinha evidência normal na Fase 5. Comparação adicional na nova build será registrada abaixo.

Arraste: Overlay, CanvasScaler 1280×720/match0,5 e EventSystem/InputSystemUIInputModule únicos. BeginDrag usa pressPosition para preservar o ponto do clique; a posição é convertida em coordenadas de mundo do Overlay. EndDrag usa centroY≥Screen.height/3 e retorna à posição inicial antes de confirmar. Nenhuma alteração na mecânica nesta fase. Logs condicionais `PE_CARD` em Editor/Development registram press/up/begin/release, posições, resolução e frame para distinguir movimento processado de um clique sem drag. Não há polling por quadro ou simulação de callback interno.

## Implementação

`CampaignSession` prepara e inicia GameplayDay, recebe sua conclusão uma única vez, mantém resultados diários, histórico e estado por cliente, soma os saldos e permite continuar somente após o resultado. A quinta conclusão produz CampaignResult e rejeita novos avanços. GameplayDay passa a aceitar número 1–5, preservando o construtor padrão do dia 1 e seus testes.

Fluxo: Menu → Nova sessão → dia 1 → resultado → Próximo dia → dias 2–4 → dia 5 → resultado final → menu → nova sessão. Não há retorno automático ao menu entre dias. O cenário, a treinadora, os movimentos próprios e a hierarquia de UI são reutilizados. As cenas FirstVisit e FirstDay permanecem fixtures de regressão; a cena de produção é Campaign.

Cotas: **3/4/5/6/6**. Cada roster preserva nível>0 como prioridade, embaralhamento/complementação documentados e remoção ao escolher. Visitas repetidas no mesmo dia são impedidas; entre dias o cliente pode voltar. Cada dia conclui o atendimento e a saída antes de checar KO; stamina zero precede esgotamento simultâneo do roster. KO encerra o dia, não toda a campanha.

Resultados: S=Σ pontos das notas; total da sessão acumula sem saturação em500; progresso=clamp(anterior+S,0,500); pontos de presentes=S+35×0, acumulados sem conversão ou recompensa. O final mostra dias, atendimentos, total, progresso, objetivo500 e objetivo alcançado/não alcançado, sem medalhas ou finais proprietários.

## Conteúdo e políticas autorais explícitas

| Adulto original | Idade / característica autoral | Preferências de tags | Prioridade inicial |
|---|---|---|---|
| Lia | 29; metódica, busca mobilidade após trabalho sentado | mobility Likes; strength Dislikes | nível1 |
| Caio | 34; direto, gosta de treinos de força | strength Likes; cardio Dislikes | nível1 |
| Nara | 27; enérgica, gosta de circuitos | cardio Favorite; mobility Ambivalent | nível1 |
| Inês | 42; paciente, valoriza precisão de postura | control Favorite; cardio Ambivalent | nível0 |
| Raul | 38; persistente, prefere força com mobilidade | strength Favorite; mobility Likes | nível0 |
| Bia | 31; curiosa, valoriza amplitude e equilíbrio | mobility Favorite; control Likes | nível0 |

Nível0 não significa bloqueio absoluto: a complementação do roster pode incluí-lo. A fórmula original de LevelUp permanece UNKNOWN. Níveis ficam fixos na sessão; contadores de visitas e histórico persistem separadamente. Nenhum contador é apresentado como LevelUp recuperado. Os três clientes prioritários preservam o dia 1 anterior, inclusive a ordem determinística da seed17.

DaySetUp original limpa scores, reseta gerenciador de habilidades e reaplica passivos; o estado completo de stamina, ordem de deck e upgrades não foi recuperado. **Adaptação mínima desta campanha:** cada dia recebe stamina inicial/máxima100, modificador1 e deck novo do mesmo catálogo próprio, mão inicial3; RNG continua na mesma sessão. Durante o dia, deck/descarte/reciclagem/stamina são compartilhados. Totais, progresso, pontos de presentes e histórico não são reiniciados entre dias. Nova sessão cria todos esses estados novamente.

A continuação entre dias informa que melhorias não estão implementadas. Não há botões fictícios de upgrade, cargas inventadas de recompra, presentes abertos, loja, galeria, save, economia permanente ou arte final.

## Validação e evidências

**49 testes aprovados, nenhum ignorado ou falho:43 EditMode e6 PlayMode.** Os39 anteriores são preservados, incluindo seus quatro arquivos sem alterações byte a byte. São9 novos casos EditMode e1 PlayMode. O PlayMode foi repetido após o ajuste tipográfico da tela diária, com6/6 novamente. Evidências ficam fora do repositório em `ProblematicSubjects_Research/Phase6`.

| Evidência privada | Resultado e limite |
|---|---|
| editmode.xml/log | 43/43: regras anteriores e campanha, cotas exatas, unicidade diária, histórico de sessão/cliente, reset diário, deck compartilhado entre visitas, saldos, KO após visita, idempotência, saturação, final e nova sessão |
| playmode.xml e playmode-final.xml/log | 6/6 em ambas: cinco testes anteriores e campanha pelos controles renderizados, continuando os cinco dias, exibindo final e reiniciando sessão; não são inputs do Windows |
| build-windows.log; Build-Windows/build-result.txt | Build Development Windows64/Mono concluída, **Succeeded/Errors=0**, somente Campaign.unity |
| baseline-altf4.log | Build intacta da Fase5 encerrou por Alt+F4 nesta revisão, sem Crash; não é correção geral da falha anterior |
| Windows/01-menu.png | Nova build renderizou menu/cenário/treinadora, mas um diálogo do Firewall ficou sobreposto; captura registra o bloqueio, não prova navegação |
| build-manifest.json | SHA-256 do launcher, assemblies próprios e level0; launcher sozinho não identifica mudanças de gameplay |
| audit.json | WorkingCopy47/47 e vídeos5/5 intactos; quatro arquivos de testes anteriores intactos; Settings/ProjectSettings/Packages iguais ao início da fase; .meta completos |

A nova build está em `C:\Users\Micro\Documents\GitHub\GameGymPjct\ProblematicSubjects_Research\Phase6\Build-Windows\ProblematicExercise.exe`; executar com Data e DLLs acompanhantes. A build reserializou UniversalRP.asset automaticamente. A versão gerada foi preservada privadamente e o arquivo do projeto restaurado da cópia exata feita antes da fase, sem mudar configurações globais ou atualizar pacotes.

**Limite Windows desta rodada:** PID26804 abriu no monitor2, 1280×720, e o Windows solicitou acesso às redes públicas/privadas. Nenhuma permissão foi concedida. O diálogo não apareceu como janela selecionável no inventário da automação; foi solicitada sua dispensa manual em Cancelar. Não houve cliques de gameplay na nova build enquanto esse diálogo permaneceu. Portanto, navegação completa da campanha, avanço de dia, arraste baixo/alto e Sair/Alt+F4 na nova build continuam pendentes de teste real. Não promover o PlayMode a evidência do executável. O percurso Windows da Fase5 continua evidência histórica do primeiro dia, não da campanha nova.

Os logs da nova build também mostram os avisos anteriores de TLS60 e shaders de pós-processamento ausentes/stripped; não foram ocultados nem atribuídos automaticamente a drivers. A janela desenha o menu, mas isso não comprova ausência de problemas durante todo o fluxo.

Situação de estabilidade: arraste sem mudança de contrato, aprovado por InputSystem em PlayMode, Windows pendente; Alt+F4 reproduziu encerramento normal no baseline desta fase, mas o crash nativo histórico permanece sem causa isolada. Não foi aplicada uma “correção” especulativa ao código próprio.

## Checkpoint e integridade

Git independente inicializado somente na pasta do projeto. Remoto tinha apenas main/f289a623c0487d9c671369a46f027241eb7defda (.gitignore), incorporado como ancestral. Branch `phase6-campaign`; checkpoint autoral da Fase5 `eb52dc9`. .gitignore herdado foi ampliado para excluir pesquisa, referências/vídeos, builds, DLLs/executáveis, saves e caches. Arquivos de produção e documentação foram preparados explicitamente e inspecionados antes do commit; nenhum arquivo proprietário foi incluído. Commit/push final será registrado após conferência remota.

Nenhuma execução de Problematic Subjects, modding ou alteração em Gym Discipline, saves, instalação ou ZIP original. A comparação de hashes é somente leitura. Nenhum histórico foi reescrito; nenhum reset/clean/force push.

## Próxima prioridade

Concluir o roteiro Windows abaixo, dispensando o diálogo em Cancelar, e revisar arraste e fechamento no menu/atividade. Depois revisar a campanha entregue antes de autorizar outro sistema. Melhorias/recompra ou save exigem contratos explícitos; galeria, loja e arte final permanecem fora desta entrega.

## Roteiro manual de estabilidade Windows

Executar a nova build em janela, 1280×720, monitor2: `ProblematicExercise.exe -screen-fullscreen 0 -screen-width 1280 -screen-height 720 -monitor 2 -logFile <caminho-privado-do-log>`.

1. Nova sessão → escolher cliente → aguardar chegada e fala → clicar no centro de uma carta. A prévia deve aparecer sem gastar stamina.
2. Segurar o botão esquerdo por pelo menos meio segundo, mover lentamente mantendo-o pressionado, soltar abaixo de altura/3: carta retorna, sem custo.
3. Repetir soltando com centro da carta acima de altura/3: atividade começa, mão inteira é descartada, custo aplicado uma vez. Não usar Confirmar para declarar esse teste aprovado.
4. Concluir atividade → reação → Continuar; conferir pontuação única e próximo cliente. Repetir em resolução diferente somente depois de registrar o resultado da condição inicial.
5. Resultado do dia → Próximo dia: conferir stamina100, mão futura de3, cota seguinte e total preservado. Repetir até final; retornar ao menu e iniciar sessão limpa.
6. Em execuções separadas, testar Sair e Alt+F4 no menu e durante uma atividade. Registrar PID, fase, horário, log e existência de Crash. Não consultar UIA enquanto fecha; esperar o processo sair e só então revisar logs.

Sem interação humana registrada, este roteiro não equivale a resultado aprovado. As limitações conhecidas permanecem até evidência suficiente.
