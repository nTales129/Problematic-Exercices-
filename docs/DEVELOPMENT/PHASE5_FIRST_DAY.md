# Fase 5 — primeiro dia completo

23/10/2026. Projeto existente Unity 6000.6.6f1. O usuário aceitou manualmente a primeira fatia e autorizou somente o dia1. Regras v0.2 preservadas; o adendo da fase5 em GAMEPLAY_SPEC e FIDELITY_MATRIX delimita o novo escopo.

## Funcionalidades implementadas

- Menu próprio com Nova sessão, Créditos e Sair. A introdução anterior não é o menu da nova build. Sair chama Application.Quit no player; no Editor, encerrar PlayMode pelo controle Stop.
- Dia1 com roster de Lia, Caio e Nara, até três atendimentos, entrada/diálogo, três cartas, prévia, confirmação, atividade, reação, avaliação, resultado e saída. Após a saída, próxima seleção sem reiniciar a sessão.
- Um deck e um ledger por dia. As visitas compartilham pilhas, stamina e pontos; descarte integral na confirmação, reciclagem quando necessária e mão curta sem carta de emergência.
- Histórico imutável por cliente, exercício, preferência agregada, nota, pontos e stamina após execução. Remoção do cliente ao escolher, sem repetição entre visitas.
- Encerramento após a visita completa e saída: stamina0 tem precedência local sobre esgotamento simultâneo dos clientes. Sem interromper exercício confirmado nem descartar seu resultado.
- Tela diária com clientes atendidos, nota/pontos de cada visita, soma diária, acumulado, progresso saturado e motivo. A demonstração termina explicitamente no dia1; não há botão de dia2.
- Sala persistente, treinadora identificável, clientes com cores próprias, braços/movimento, entrada/saída, HUD compacto, cartas com hierarquia e reação de resultado antes do próximo roster. Geometria própria, sem arte final.
- Retorno ao menu e nova sessão com estado novo, reutilizando a mesma hierarquia; sem duplicar Canvas ou EventSystem.

## Arquitetura e fórmulas

`GameplayDay` possui ClientRoster, ExerciseDeck e VisitResources; cada FirstVisit recebe o deck e o ledger compartilhados. O fluxo antigo permanece como fixture de regressão. A produção usa `Assets/ProblematicExercise/Scenes/FirstDay.unity`; a cena FirstVisit anterior permanece preservada.

ClientRoster prioriza nível>0, embaralha/toma cota e complementa com os restantes sem duplicar, seguindo o predicado confirmado e a composição fortemente inferida. Ao sobrar um cliente, existe uma opção; não fabricar outra para cumprir a amostragem de2ou3. Dados próprios têm nível inicial1. A fórmula original de LevelUp continua UNKNOWN: não foi inventado incremento. Atendimentos completos são contados separadamente.

As regras são calculadas no domínio, nunca na UI:

| Regra | Comportamento |
|---|---|
| Preferência | Likes+1, Dislikes+1, Ambivalent0, Favorite+25; limites recuperados |
| Nota | D/C/B/A/S/W; raridade e categoria agregada, Favorite→W |
| Pontos | 10/14/18/22/26/34; uma aplicação antes de Continuar |
| Stamina | Custos10/25/40/55; produto float32 negativo, floor e saturação; não exigir saldo suficiente |
| Progresso | S=soma das notas convertidas em pontos; novo=clamp(T+S,0,500) |
| Pontos de presentes | S+P×E, P35/E0; mantidos como pontos, sem abrir presentes ou criar economia |
| Perfil | Cotas3/4/5/6/6 como dados imutáveis; somente primeiro índice executável |

Não foram implementados campanha restante, tutorial, turnos/eventos especiais, recompra, loja, galeria, save, medalhas, recompensas ou arte definitiva.

## Testes automatizados

**39 testes aprovados: 34 EditMode e cinco PlayMode.** Os 28 anteriores permanecem, sem editar seus dois arquivos de testes. Nenhum teste ignorado.

| Grupo | Cobertura e evidência privada |
|---|---|
| 26 EditMode anteriores | Fórmulas, fronteiras, visita, descarte e stamina; incluídos em Phase5/editmode.xml |
| 8 EditMode novos | Três visitas; pilhas/saldos persistentes; clientes únicos; reciclagem; KO após resultado/saída; precedência KO; filtro/complemento de roster; roster curto; progresso/saldo; nova sessão; comandos repetidos |
| 2 PlayMode anteriores | Atendimento individual e arraste baixo/alto, preservados |
| 3 PlayMode novos | Menu/créditos/três visitas/resultado/reinício limpo; stamina/reação/resultado diário; arraste rápido sem quadro intermediário |
| Resultado final PlayMode | Phase5/playmode-order-final.xml:5/5, sem falhas; inclui chegada→fala→mão antes do arraste rápido |

No fluxo automatizado de menor custo: Caio C14, Nara B18, Lia B18; total50, stamina final55 e progresso50/500. No fluxo de maior custo: total86, stamina0, motivo OutOfStamina, com três resultados registrados antes da tela diária. Caso sintético de stamina inicial5 prova encerramento antecipado após uma única visita, preservando os dois clientes restantes.

Os testes de UI enviam MouseState pelo Input System, passam por EventSystem/GraphicRaycaster e acionam controles renderizados. Não invocam callbacks de botão para simular navegação. O ajuste de foco do Editor é temporário em memória e restaurado no teardown. Os testes EditMode usam os comandos de domínio; isso não é apresentado como prova de cliques.

Viewport do Editor batch:640×480. Capturas de apresentação: renderização offscreen do Canvas em1280×720, com atualização das máscaras antes do render e restauração do Overlay antes dos inputs. Essas imagens não são screenshots de janela e não provam interação no executável.

## Build e interação Windows

Nova build de desenvolvimento Windows x86_64/Mono em `ProblematicSubjects_Research/Phase5/Build-Windows/ProblematicExercise.exe`, com Data/DLLs acompanhantes. A lista de cenas da build é explícita e contém somente FirstDay; não foi trocada a lista global de BuildSettings. Build inicial passou com zero erros; a versão final inclui a correção de arraste rápido.

**Fluxo completo validado na janela da build de entrega**, inclusive após separar chegada e fala, com Computer Use/SendInput, sem callbacks internos. A janela foi selecionada pelo caminho do executável e observada antes de cada ação. Capturas reais e sequência com timestamps estão em `Phase5/WindowsDelivery`; a rodada anterior em WindowsFinal conserva os testes de créditos e reinício. Não são os renders offscreen do Editor.

| Etapa observada no player | Resultado |
|---|---|
| Menu → Créditos → Voltar | Funcionou na versão final |
| Nova sessão | Três clientes, stamina100, pontos0 |
| Caio / Postura | Confirmação pelo botão; custo10, C14; próxima seleção conserva stamina90/pontos14 |
| Nara / Remada | Segunda mão com as outras cartas; custo25, B18; próxima seleção conserva stamina65/pontos32 e só Lia disponível |
| Lia / Mobilidade | Mão reciclada; custo10, B18; total50 e stamina55 |
| Saída → resultado diário | Três clientes, C/B/B, soma50, acumulado50, progresso50/500, motivo clientes atendidos; HUD3de3 |
| Voltar ao menu → Nova sessão | Retorna a stamina100, pontos0, três clientes e contador0de3 |
| Sair no menu | Instância separada PID25452 e instância de entrega após o dia completo PID25580 encerraram; logs sem Crash nesses caminhos. `quit-result.json` e `delivery-quit-result.json` confirmam processos ausentes |
| Encerramento por stamina | Validado em EditMode/PlayMode, **não repetido na janela Windows** |
| Arraste na janela | `sky.drag` não confirmou a carta, mesmo após a correção do offset; **verificação manual pendente**. O percurso Windows usou selecionar + Confirmar |

O comando de arraste nativo e o atalho de fechar não têm resultado inferido somente pelo retorno da ferramenta. A tentativa inicial de capturar uma janela ocluída mostrou conteúdo fora do alvo; não houve input nesse estado. O alvo foi ativado e recapturado antes dos cliques, e a imagem de evidência do menu foi substituída pela captura correta do jogo.

Foram detectados dois monitores. A execução interativa usa `-screen-fullscreen 0 -screen-width 1280 -screen-height 720 -monitor 2`. A documentação oficial da Unity descreve seleção de monitor por índice a partir de1 e os argumentos de resolução/janela: [Unity Player command-line arguments](https://docs.unity3d.com/6000.0/Documentation/Manual/PlayerCommandLineArguments.html). A captura da janela inicial mostrou origem X=-1920 e1282×752 incluindo moldura, compatível com client1280×720 no monitor secundário.

## Problemas encontrados

1. O resultado diário inicial deixava o HUD em “2 de3”, embora o resultado tivesse três clientes. Corrigida a atualização do HUD após finalizar a saída; teste verifica “3 de3”.
2. Na captura offscreen, a máscara podia manter culling do viewport anterior e ocultar a treinadora. A captura agora aguarda a atualização do Canvas/máscaras antes do render. Nenhuma alteração de regras por causa da captura.
3. A abertura oculta do player não expunha janela selecionável. Foi encerrada somente a instância da tarefa e relançada uma janela interativa no segundo monitor.
4. **Arraste rápido:** após uma tentativa `sky.drag` sem confirmação no player, a inspeção identificou fragilidade no offset: BeginDrag pode receber posição já distante do clique. Corrigido para usar pressPosition; novo teste com um único salto grande passa. A ferramenta Windows continuou sem confirmar o drag; não se atribui essa limitação definitivamente ao offset nem se declara arraste nativo aprovado. Selecionar e Confirmar por clique funcionaram em todos os atendimentos.
5. Avisos de TLS60 dos serviços Unity, interface de debug D3D12 e limpeza de ComputeBuffer ao fechar foram observados nos logs do ambiente/pipeline. Não foram alterados certificados, segurança do Windows ou pacotes para suprimi-los.
6. **Falha nativa ao fechar por Alt+F4:** uma instância anterior do dia completo registrou `Crash!!!` durante shutdown do Input System/UnityPlayer, depois do teste de reinício. A causa não foi isolada e não foi atribuída ao driver ou às regras de gameplay. O botão Sair foi testado separadamente e após o dia completo na build de entrega, encerrando normalmente. Portanto, não declarar estabilidade geral de todos os modos de fechamento. As instâncias já terminaram e nenhuma janela de teste ficou aberta.
7. A marca Development Build pode sobrepor parte da faixa inferior de notas; os resultados individuais e a tela diária permanecem legíveis. É uma limitação da apresentação de desenvolvimento, não erro de soma.
8. A revisão final separou a fala de entrada da caminhada no modo diário: chegada0,8s, depois fala placeholder0,6s, só então a mão. O teste verifica a mão ainda oculta durante a fala. Esses tempos são autorais, não constantes recuperadas; a fixture legada preserva seu comportamento para regressão.

## Divergências e pendências

- Stamina100, nível inicial1, catálogo de seis exercícios, preferências dos três adultos e seed17 são fixtures autorais/provisórias, não dados recuperados do original.
- System.Random/Fisher–Yates permanece adaptação técnica; não se alega mesma sequência/probabilidade do original.
- Tempos de entrada/saída0,8s, fala após chegada0,6s, reação0,4s, composição, movimento geométrico, diálogo e botão alternativo Confirmar são próprios. O limiar de arraste continua centroY≥altura/3.
- Somente stamina do jogador é fonte de KO nesta etapa. Composição completa de outras fontes e eventos especiais não é alegada equivalente.
- Filtro/complemento do roster preserva a confiança da pesquisa. Efeitos completos de LevelUp, tutorial e todos os overrides permanecem pendentes.
- Não há valor inventado de cargas de recompra, conversão de presentes, prêmios, save ou economia permanente.
- Só dia1 pode ser iniciado; os dados dos outros dias não são uma implementação da campanha.
- Arraste manual na build e fechamento por Alt+F4 precisam de revisão específica. Uma comparação restrita de backend gráfico/encerramento pode ser feita em etapa posterior; não foi iniciada investigação ampla nesta entrega.

## Execução e revisão

Abrir a build em janela usando os argumentos acima, ou abrir FirstDay.unity no Editor e pressionar Play. O menu **Problematic Exercise → Open First Day** prepara/abre a cena. O menu antigo Open First Visit serve à regressão da fatia anterior.

Roteiro: Nova sessão → cliente → aguardar entrada → selecionar/arrastar carta → Confirmar, se necessário → Concluir exercício → aguardar reação → Continuar → próximo cliente. Após o último ou KO válido, conferir resultado e Voltar ao menu. Nova sessão deve voltar a stamina100, pontos0 e três clientes. Sair deve fechar o player. Não há save de gameplay; fechar descarta a sessão de testes.

Evidências privadas em `ProblematicSubjects_Research/Phase5`: XML/logs de testes, `build-windows-delivery.log`, `Build-Windows/build-result.txt`, CapturesFinalOrder (offscreen), WindowsCaptures (tentativa inicial), WindowsFinal (créditos/reinício/primeiro percurso), WindowsDelivery (percurso na versão de entrega/steps.json), logs Windows, `build-manifest.json` e `delivery_audit.json`. A captura real `WindowsDelivery/08-day-result.png` mostra o fechamento diário da entrega; `WindowsFinal/16-fresh-session.png`, o reinício na rodada anterior. O manifest inclui assemblies próprios e cena, pois o executável launcher Unity pode manter o mesmo hash entre builds. A build anterior em FirstSlice permanece preservada.

## Integridade e estado final

A auditoria confirmou WorkingCopy47/47 e vídeos5/5 iguais aos hashes anteriores. Os dois arquivos que contêm os 28 testes legados permanecem byte a byte iguais ao início da fase5. Nenhuma alteração no Gym Discipline, na instalação/ZIP do original ou em saves pessoais; nenhum commit/push. Não foram instalados pacotes nem outras versões do Editor.

Houve cópia de segurança dos scripts próprios, documentação, Assets/Settings e ProjectSettings antes das alterações. A build regravou UniversalRP.asset automaticamente; as versões geradas foram preservadas em `BuildGeneratedSettings` e `BuildGeneratedSettingsDelivery`, e o arquivo de projeto foi restaurado a partir da cópia exata desta fase. Settings e ProjectSettings preexistentes ficaram iguais ao início da fase5; BuildSettings global não foi trocado. A build inclui a serialização processada pela Unity no momento de sua geração.

Estado: primeiro dia funcional e revisável pelo usuário, com39 testes aprovados e percurso real Windows concluído. Não é declaração de estabilidade de release: drag manual e fechamento por atalho continuam pendentes. Próxima revisão deve validar esta entrega e essas limitações antes de expandir campanha ou arte.
