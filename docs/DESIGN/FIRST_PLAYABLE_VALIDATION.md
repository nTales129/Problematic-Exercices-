# Primeiro atendimento — implementação e validação

Registro histórico da primeira fatia. O usuário informou na fase5 que a testou manualmente e autorizou o primeiro dia completo. A entrega atual usa `FirstDay.unity`; consultar `docs/DEVELOPMENT/PHASE5_FIRST_DAY.md`. A build antiga e as evidências abaixo foram preservadas.

23/10/2026. Escopo: jogo independente, uma visita com código e placeholders próprios. Spec v0.2 e FIDELITY_MATRIX foram gravadas antes dos arquivos de runtime. A implementação não reutiliza código descompilado nem conteúdo do jogo investigado.

## Resultado

**Implementado e testado em PlayMode:** Início → cliente → entrada → três cartas → prévia → confirmação → atividade → avaliação → resultado → saída → atendimento concluído. Há retorno ao início para repetir a fatia com estado novo em memória. O atendimento concluído não representa um dia completo.

| Verificação | Resultado e limite |
|---|---|
| Compilação Unity | Passou no Editor 6000.6.6f1; erro inicial de uso de propriedade interna TMP corrigido |
| EditMode | **26/26 passaram**, 0 falhas, 0 ignorados; fórmulas, fronteiras, deck e transições |
| PlayMode | **2/2 passaram**, 0 falhas, 0 ignorados na execução final; mouse virtual pelo Input System/EventSystem e GraphicRaycaster |
| Fluxo por botões | Iniciar, cliente, selecionar carta, Confirmar, Concluir exercício, Continuar e Voltar ao início exercitados |
| Arraste | Soltura baixa retorna sem custo; acima do limiar confirma, descarta mão e cobra |
| Estado e resultado | Três cartas descartadas; custo e score únicos; resultado aplicado antes de Continuar; saída não duplica pontos |
| Build | Windows x86_64, Development, Mono, **Succeeded / Errors=0**, somente FirstVisit na lista explícita de cenas |
| Executável | Inicialização em segundo plano/janela oculta, Direct3D 12; sem exceção de aplicação observada no log. Processo encerrado após smoke |
| Navegação no executável | **Não testada**; os cliques acima são evidência de PlayMode, não do `.exe` |
| Visuais | Cinco PNGs do Canvas próprio renderizado offscreen, conferidos; sem recursos do original |

Caso observado na execução final: Lia, Série avançada, raridade3. Preferência por força Dislikes contribui +1, agregada Likes; nota W e34 pontos. Stamina100→45. Os testes de domínio também cobrem Likes e Dislikes isolados, t20/t21, Favorite antes da raridade, tabela completa e custo reduzido25×0,7f com delta-18.

## Evidências privadas preservadas

Raiz: pasta irmã `ProblematicSubjects_Research/FirstSlice/`. Não copiar esta pasta para Assets nem distribuir junto ao código público.

| Arquivo | Evidência |
|---|---|
| editmode.xml / editmode.log | 26 casos passando no Test Framework |
| playmode-offscreen.xml / playmode-offscreen.log | Dois cenários finais passando; início UTC17:36:44, fim17:36:48 |
| Captures3/start.png | Tela inicial |
| Captures3/hand.png | Três cartas antes de confirmar; stamina100 e pontos0 |
| Captures3/activity.png | Atividade própria após confirmação |
| Captures3/result.png | Lia, W,34 pontos, stamina45 |
| Captures3/complete.png | Saída concluída |
| build-windows.log | Build Finished, Result: Success |
| Build-Windows/build-result.txt | Succeeded, Errors=0 |
| standalone-startup.log / standalone-process.json | Inicialização do executável próprio; PID29012, posteriormente encerrado |
| delivery_audit.json | Integridade da WorkingCopy, vídeos, arquivos preexistentes e inventário dos novos arquivos |
| design_v0.1/ | Especificação/arquitetura/roadmap anteriores preservados |

SHA-256 do executável gerado: `ac52a872fc9dd41f49f03a796a85af890447405b28cc0167eef7679267ff04eb`.

## Condições e limitações da validação

O CLI instalado é Unity1.0.0-beta.13. A consulta inicial de atualização falhou por DNS; consulta posterior sinalizou beta.14. Não houve atualização ou reinstalação de ferramentas. Nenhum pacote novo de gameplay foi adicionado. Foram importados os TMP Essentials já fornecidos pelo pacote uGUI instalado, incluindo fonte Liberation Sans e sua licença OFL.

O sandbox bloqueou a primeira inicialização do licenciamento e o Editor falhou antes de compilar. A execução autorizada fora do sandbox usou o mesmo Editor instalado e passou. Não houve alteração de segurança do Windows, coleta de senhas ou contorno de proteção. Toda execução desta entrega foi do projeto independente sob Micro; **o jogo original, BepInEx e PE_Tester não foram executados**.

O Editor batch utilizou viewport **640×480**, apesar dos argumentos1280×720. O Canvas usa referência1280×720 com escala. Os cliques e arrastes finais passaram a640×480. A comparação com Screen.height/3 foi exercitada nesse viewport; outras resoluções ainda precisam de verificação.

A captura de backbuffer ScreenCapture não produziu PNG no Editor batch. A solução final renderiza o **mesmo Canvas** temporariamente por uma câmera para RenderTexture, grava PNG e restaura Overlay antes dos inputs. Os PNGs são640×480 e evidência da renderização da UI; não são screenshots de uma janela interativa nem uma gravação contínua do loop. O player foi iniciado com parâmetros1280×720, mas essa resolução e a navegação nele não foram verificadas visualmente.

Os testes ajustam apenas em memória o roteamento de entrada para o Editor sem foco, restaurando backgroundBehavior e editorInputBehaviorInPlayMode no teardown. Não chamam Button.onClick, callbacks privados, Select/Confirm do domínio ou ExecuteEvents para fingir navegação. A geração das entradas é feita por QueueStateEvent em um Mouse virtual. As asserções leem o modelo depois do processamento normal da UI.

## Falhas encontradas e resolução

| Tentativa preservada | Problema | Resolução / estado |
|---|---|---|
| prepare-editor.log | Licenciamento sem acesso no sandbox; crash do Editor | Encerrado somente PID criado pela tarefa; execução fora do sandbox passou |
| prepare-editor-full.log | CS0117 por propriedade interna TMP | Usar verificação do asset; compilação seguinte passou |
| prepare-editor-fixed.log / prepare-font.log | Importação TMP assíncrona, encerramento precoce; reload removeu callback de espera | Recursos importados; ferramenta batch agora retoma espera via SessionState após reload. Caminho de retomada novo não foi reexercitado com uma importação limpa |
| playmode.xml | Falta de foco impediu primeiro clique; cena de teste duplicada | Roteamento temporário e fixture sem criar cena duplicada |
| playmode-focus-fixed.xml | Captura de backbuffer ausente interrompeu testes | Captura offscreen; execução final2/2 passou |
| Logs de Editor/build/player | Avisos de TLS60 de serviços Unity; interface de debug D3D12 indisponível; shaders HDRP do pacote sem suporte no URP | Não bloqueantes nesta execução. Não alterar Windows/certificados nem trocar pipeline para suprimir avisos |

Relatórios das tentativas falhas foram mantidos, separados do XML final. O smoke do `.exe` foi somente de inicialização; o encerramento foi pelo PID criado pela tarefa, não um teste do botão de sair.

## Arquivos e integridade

Código próprio em `Assets/ProblematicExercise/{Domain,Runtime,Editor,Tests}`; cena `Assets/ProblematicExercise/Scenes/FirstVisit.unity`. As regras ficam em Domain; UI não recalcula nota nem contém recursos extraídos. Metas foram geradas pela Unity. `Assets/TextMesh Pro` contém os recursos do pacote Unity, não material do jogo pesquisado.

A conferência após a build manteve **47/47 arquivos da WorkingCopy** e **5/5 vídeos** iguais aos hashes anteriores. SampleScene, manifest de pacotes e EditorBuildSettings continuam iguais. Nenhum original, ZIP, save pessoal ou Gym Discipline foi usado como destino; nenhum commit/push.

Dos 62 arquivos preexistentes de Assets/Packages/ProjectSettings, a Unity regravou três durante a build. Foi possível restaurar `Assets/Settings/UniversalRP.asset` a partir do template instalado **somente após confirmar correspondência exata com o hash anterior à tarefa**; a versão regravada pela build foi preservada em `FirstSlice/build-generated-settings`. Assim, 60 dos 62 arquivos permanecem iguais ao baseline final da pesquisa.

Permanecem regravados automaticamente `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset` e `ProjectSettings/UnityConnectSettings.asset`. O template não corresponde aos hashes anteriores desses dois; não se tentou restaurá-los por suposição. Não houve edição manual de seus parâmetros. A lista global de cenas não foi alterada. A build registrada usa as serializações geradas pela Unity durante seu próprio processamento; os testes PlayMode ocorreram antes dessas regravações de build.

## Como experimentar

No Editor6000.6.6f1, abrir `Assets/ProblematicExercise/Scenes/FirstVisit.unity` e entrar em Play. Se precisar preparar uma cópia limpa do projeto, usar **Problematic Exercise → Prepare First Visit** para importar os recursos TMP e criar a cena; aguardar importação terminar. O menu **Open First Visit** abre a cena dedicada e protege alterações de cena não salvas.

A build local está em `ProblematicSubjects_Research/FirstSlice/Build-Windows/ProblematicExercise.exe`. Manter junto a pasta Data e DLLs geradas. Não é uma distribuição pública. Para conferir em janela1280×720, executar o próprio `.exe` com `-screen-fullscreen 0 -screen-width 1280 -screen-height 720`.

Roteiro manual complementar do executável:

1. Iniciar atendimento e escolher um cliente.
2. Esperar entrada e verificar três cartas. Clicar numa carta: stamina e pontos permanecem iguais.
3. Arrastar pouco para cima e soltar abaixo de um terço da tela: carta retorna sem cobrança.
4. Arrastar acima do limiar ou selecionar e Confirmar: cartas somem e stamina é cobrada uma vez.
5. Observar o movimento, Concluir exercício e conferir nota/pontos.
6. Continuar, esperar saída e Voltar ao início. Não deve iniciar dia/campanha nem criar save de gameplay.

Não avançar automaticamente para dias, personagens finais, conversão completa ou outros sistemas. Próximo passo de revisão: avaliar esta fatia e confirmar visualmente o executável em1280×720; regras UNKNOWN e adaptações continuam explícitas na matriz.
