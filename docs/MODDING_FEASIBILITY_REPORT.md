# Problematic Exercise — viabilidade de mod visual IL2CPP

Data: 23/10/2026, America/Sao_Paulo. Investigação local da versão Windows fornecida, identificada na tela como `1.4.0-dev-5 DEV-RELEASE`.

**Atualização da etapa de estabilidade (23/10/2026):** a recomendação C deste relatório foi autorizada pelo usuário. A nova execução foi impedida na verificação obrigatória de isolamento de perfil; a matriz A/B/C não foi executada. Os resultados abaixo continuam sendo exclusivamente os do primeiro experimento. A recomendação atual é **B — resolver o isolamento técnico antes de continuar**, segundo as alternativas da nova solicitação. Consulte [MODDING_STABILITY_TEST.md](MODDING_STABILITY_TEST.md). Não foi demonstrada inviabilidade da abordagem de modding.

## Conclusão

**Foi possível carregar BepInEx IL2CPP, substituir um fundo estático por uma imagem autoral em memória e restaurar o sprite original na mesma execução.** O experimento não regravou arquivos preexistentes do jogo. Isso demonstra uma via técnica real para um mod visual pequeno.

**Ainda não está demonstrado que uma conversão temática completa seja mais eficiente que o desenvolvimento independente.** A navegação por cliques, os estados durante uma partida e a substituição de personagens animados não foram verificados. Além disso, surgiram incompatibilidades de APIs geradas pelo interop e uma anomalia visual nas capturas.

**Recomendação: C — realizar mais um experimento técnico específico**, descrito ao final. Não iniciar automaticamente o mod completo nem a reconstrução com base neste teste.

## Escopo e isolamento

As informações de engine, backend, metadados e Addressables fornecidas pelo usuário foram aceitas como premissas. Não foi refeita a identificação básica da engine.

Locais utilizados:

- `ProblematicSubjects_Research/WorkingCopy`: fonte para inspeção, sem modificações.
- `ProblematicSubjects_Research/Experiments/LoaderCopy`: segunda cópia integral da WorkingCopy, exclusiva para executar o loader e o plugin.
- `ProblematicSubjects_Research/Tools`: ferramentas portáteis e dependências locais.
- `ProblematicSubjects_Research/Evidence`: inventários, hashes, logs e capturas locais.
- `ProblematicSubjects_Research/Experiments/VisualProbe`: código original do diagnóstico e gerador da imagem autoral.

A pasta `Original` não foi utilizada para a inspeção. Nenhum recurso extraído, cena ou código do jogo foi importado para o projeto autoral. No projeto Problematic Exercise, a entrega é este documento. Gym Discipline não foi alterado. Não houve commit, push, publicação, distribuição ou tentativa de contornar DRM.

A cópia da instalação **não equivale a um perfil de usuário isolado**. O Unity utilizou o perfil Windows corrente. Antes da primeira execução, não foram encontrados arquivos na pasta persistente esperada nem entradas na chave de preferências consultada. Após as execuções, a pasta permaneceu sem arquivos, mas surgiram 22 entradas no registro, referentes a vídeo, sessão, identificadores do Unity e idioma. Não foi iniciado um jogo nem selecionada uma operação de save. Não se deve interpretar esse resultado como um teste de compatibilidade com saves existentes, outros perfis, Steam Cloud ou armazenamento em outros locais.

Após a preferência adicional do usuário, as execuções solicitaram `-monitor 2`, sem ativação inicial da janela. O plugin habilitou temporariamente `Application.runInBackground` na cópia de teste para permitir observação em segundo plano. A posição efetiva da janela no segundo monitor não foi confirmada pela ferramenta de automação.

## Ferramentas e compatibilidade observada

| Ferramenta | Versão utilizada | Resultado nesta instalação | Limite da confirmação |
|---|---|---|---|
| AssetRipper | 2.0.0, Windows x64 | Importou e processou um bundle de UI; reconheceu os assets como Unity 6000.3.18f1 | Leitura de bundle confirmada. Não foi testada reconstrução/exportação de projeto nem análise IL2CPP completa |
| AssetStudioMod, aelurum/AssetStudio | 0.19.0, CLI .NET 8 | Modo `info --load-all` leu o mesmo bundle: 111 objetos, incluindo 98 sprites, 4 texturas e 1 atlas | Leitura dessa amostra confirmada. Não comprova todos os formatos nem suporte próprio a metadados IL2CPP 39 |
| BepInEx Unity.IL2CPP win-x64 | 6.0.0-be.790, commit `4e98359b51c48f103f948c57faca3be20027407a` | Gerou interop, chegou ao menu, carregou o plugin original e executou troca/restauração | Compatibilidade parcial e efetivamente executada com esta combinação; APIs específicas apresentaram falhas |
| Cpp2IL / Il2CppInterop | Componentes incluídos no pacote BepInEx acima | Primeiro bootstrap reconheceu metadados 39 e concluiu geração; foram produzidas 136 DLLs de interop | Não foram instaladas versões avulsas; não se deve substituir componentes individualmente sem outro teste |
| UnityPy | 1.25.4 | Indexou em memória os 21 bundles e `data.unity3d` | Alguns MonoBehaviours do arquivo principal não puderam ser interpretados integralmente |
| Roslyn | C# 4.10.0, SDK 8.0.318 já instalado com o Editor | Compilou o plugin original contra as DLLs do loader e do interop | O Editor instalado não foi usado para abrir/exportar um projeto reconstruído |
| Python / Pillow | Python 3.12; Pillow no ambiente local | Inventários, hashes, imagem de teste e comparação de capturas | Não usados para extrair imagens proprietárias |

O pacote estável BepInEx 5 não foi escolhido para IL2CPP. A documentação oficial direciona essa instalação para a distribuição IL2CPP específica. [Instalação oficial IL2CPP](https://docs.bepinex.dev/master/articles/user_guide/installation/unity_il2cpp.html).

As ferramentas de leitura de assets não precisam necessariamente interpretar `global-metadata.dat` para listar sprites e texturas com dados de serialização disponíveis. Portanto, ler um bundle não confirma compatibilidade com metadados 39. Essa parte foi exercitada pelo bootstrap do BepInEx.

O AssetRipper avisou que faltava a dependência `CAB-951c2a189c12f125fe42cc5e26e5b5a0` ao abrir somente a amostra de UI. Também informou backend `Unknown` nessa importação isolada; isso não contradiz o backend IL2CPP já conhecido. Não foi acionado `Export All Files`.

Fontes das distribuições consultadas: [AssetRipper 2.0.0](https://github.com/AssetRipper/AssetRipper/releases/tag/2.0.0), [AssetStudioMod 0.19.0](https://github.com/aelurum/AssetStudio/releases/tag/v0.19.0), [BepInEx build 790](https://builds.bepinex.dev/projects/bepinex_be/790). Os URLs, versões e informações dos releases ficaram registrados em `Evidence/official_releases.json` e `Evidence/bepinex_source.txt`.

## Organização dos recursos

A inspeção listou objetos e referências; não realizou exportação em massa de imagens, áudio, cenas ou código.

O catálogo é binário (`StreamingAssets/aa/catalog.bin`). A busca de strings identificou chaves e nomes legíveis, mas **não constituiu um parser completo das dependências do catálogo Addressables 2.9.1**. As associações abaixo também usam GUIDs dos containers efetivamente lidos nos bundles.

| Local | Organização observada |
|---|---|
| `data.unity3d` | Menus e interfaces, materiais, fontes, recursos de inicialização e objetos de cenas. Contém o sprite escolhido, em `sharedassets1.assets`, PathID 38 |
| `b78282e464e3df3af87872317fe3b3b1.bundle` | Recursos globais de UI: atlas `UI_Global_Sprites`, sprites, materiais e shaders |
| `24d353382e5bc1244bc85bc94d4a8b45.bundle` | `SharedStageAssets`, bibliotecas de variações, atlases e sprites compartilhados entre estágios |
| `9c1d36678f140b452c6721d68c96bb31.bundle` | Grupo associado a Detention: objetos de cenário, personagens/atividades, interfaces, configurações, animações e apresentações finais |
| `b404c639e91f26585256aca1fe06247c.bundle` | Grupo associado a Infirmary, com organização semelhante |
| `c4df515945152a8a8d1618c87f6d9968.bundle` | Grupo associado a Gym: configurações, cenário, personagens, atividades, sprites e animações |
| `951c2a189c12f125fe42cc5e26e5b5a0.bundle` | 118 objetos MonoScript, utilizados como referências por componentes de outros bundles |
| Demais bundles | Incluem tabelas localizadas e TextAssets. O papel completo de cada payload binário não foi estabelecido |

Totais de objetos serializados encontrados, somando bundles e arquivo principal:

| Tipo | Contagem |
|---|---:|
| Sprite | 1.081 |
| Texture2D | 164 |
| SpriteAtlas | 103 |
| GameObject | 2.654 |
| SpriteRenderer | 420 |
| AnimationClip | 270 |
| AnimatorController | 96 |

Esses números **não são quantidades de artes originais únicas ou de trabalho a produzir**. Há objetos repetidos entre arquivos, recursos compartilhados, variantes e material de UI. Não foi feita deduplicação semântica. Os três bundles de estágio contêm, respectivamente, 59, 103 e 70 AnimationClips; o arquivo principal contém 38.

Muitas texturas são páginas de atlas, com nomes `sactx-*` e compressão BC7. Trocar uma textura inteira pode afetar vários sprites. O experimento evitou esse risco atribuindo um novo Sprite a um único renderer.

### Dependências relevantes para uma futura conversão

Foram observados campos serializados, sem reprodução de código descompilado:

- `CharacterSpriteVariationLibrary` possui `configCollection`.
- `ActivityPrefabLibrary` possui `prefabCollection`.
- `ActivityFaceSwappableCharacter` referencia um renderer, bibliotecas de variação e `headTransform`.
- `ActivityPoseFixedCharacter` relaciona `characterId` e `headTransform`.
- `PlayableActivity` relaciona configuração, posição de diálogo, área e renderers adicionais.
- `StagePlayConfig` reúne `worldScenarioPrefab`, `activitySystemConfig`, `daysConfig`, configuração de diálogos, score e assets compartilhados.

Isso sustenta a hipótese de uma apresentação composta por partes e bibliotecas de variação. Não demonstra que basta trocar uma imagem para converter um personagem completo. Preservar caminhos de animação, pivôs, escala, máscaras, ordenação, variantes e ciclo de carregamento será necessário.

Evidência local: [inventário de objetos](../../ProblematicSubjects_Research/Evidence/asset_inventory.json) e [campos dos componentes](../../ProblematicSubjects_Research/Evidence/visual_component_fields.json).

## Experimento mínimo executado

Alvo: `MainMenu/Background`, componente `SpriteRenderer`, sprite `Main_Menu_BG_Wide`, retângulo original 2520 × 1080 e 100 pixels por unidade. A instância registrada na execução bem-sucedida foi `16938`.

O plugin `Single Visual Probe 0.1.0` foi escrito do zero. Não referencia a assembly de lógica específica do jogo, não aplica patches Harmony e não acessa saves, progressão ou regras.

Procedimento:

1. Inicializar a cópia com BepInEx e localizar o renderer pelo nome do sprite.
2. Preservar a referência original em memória e capturar o menu antes da troca.
3. Ativar o marcador `BepInEx/plugins/VisualProbe/visual-test.enabled`.
4. Ler uma imagem autoral de teste em formato RGBA, criar Texture2D/Sprite e atribuí-lo somente ao renderer selecionado.
5. Capturar o resultado. Desativar o marcador renomeando-o para `visual-test.disabled`.
6. Reatribuir a referência original ao mesmo renderer e capturar a restauração.

A imagem autoral tem 1260 × 540 pixels, mesma proporção do original. O novo sprite usa 50 pixels por unidade, preservando o tamanho em unidades de mundo, o pivô e a borda. A produção do raster partiu apenas de cores, grade, diagonais e texto originais, sem pixels extraídos do jogo.

**Resultado:** troca e restauração executadas, registradas em log e verificadas visualmente nas capturas. O fundo quadriculado apareceu no lugar da composição estática original.

| Evidência local | Conteúdo |
|---|---|
| [01_original.png](../../ProblematicSubjects_Research/Evidence/01_original.png) | Menu com o fundo original |
| [02_replaced.png](../../ProblematicSubjects_Research/Evidence/02_replaced.png) | Imagem autoral substituindo o fundo |
| [03_restored.png](../../ProblematicSubjects_Research/Evidence/03_restored.png) | Composição original após restauração |
| [bepinex_visual_success.log](../../ProblematicSubjects_Research/Evidence/bepinex_visual_success.log) | TARGET, APPLIED e RESTORED para a mesma instância |
| [integrity_verification.json](../../ProblematicSubjects_Research/Evidence/integrity_verification.json) | Hashes dos 47 arquivos preexistentes conferidos |
| [screenshot_comparison.json](../../ProblematicSubjects_Research/Evidence/screenshot_comparison.json) | Diferenças entre os quadros capturados |

O quadro modificado difere do inicial em aproximadamente 79,81% dos pixels. O restaurado difere em 0,127%; não é uma restauração pixel a pixel idêntica do frame, embora o sprite original tenha sido reatribuído e o fundo restaurado visualmente. Há elementos animados e a anomalia dos rótulos descrita abaixo.

Os arquivos do jogo mantiveram seus hashes tanto na WorkingCopy quanto na LoaderCopy. A cópia de teste recebeu apenas arquivos adicionais de loader, interop, configuração e plugin. As DLLs de interop são material gerado localmente para execução e não devem ser copiadas para o projeto autoral nem distribuídas.

## Problemas encontrados

1. **Alvo inicial incorreto:** a primeira implementação procurava um `Image`. O inventário em execução mostrou que o fundo usa `SpriteRenderer`. A implementação foi corrigida com essa evidência.
2. **Interop parcial:** o bootstrap avisou `Class::Init signatures have been exhausted, using a substitute!`. A primeira geração também relatou membros que não pôde restaurar. Chegar ao menu não garante todas as APIs.
3. **APIs com Span:** `ScreenCapture.CaptureScreenshot(string)` e `ImageConversion.LoadImage` falharam com `MissingMethodException` de `Il2CppSystem.ReadOnlySpan<T>.GetPinnableReference`. Falhas registradas em `bepinex_screencapture_failure.log` e `bepinex_png_span_failure.log`. A solução restrita ao experimento foi captura por textura/EncodeToPNG e leitura do raster RGBA autoral via SetPixel/Apply. Esse preenchimento por pixel é diagnóstico, sem medição de desempenho e inadequado como pipeline final de produção.
4. **Composição em camadas:** o sprite chamado de fundo também incorpora a maior parte da personagem estática. Olhos e caneta são renderers separados. Após trocar só o fundo, esses elementos permaneceram sobre a grade. Uma conversão exigiria tratar o conjunto das camadas, mesmo nessa tela.
5. **Rótulos ausentes:** SETTINGS e SAVEFILE aparecem no quadro inicial, mas não nas capturas posteriores da troca e da restauração. Não foi determinado se isso decorre da captura fora do fim do frame, de renderização em segundo plano, de timing ou de outra regressão. A reversão do fundo não comprova a reversão dessa anomalia.
6. **Automação de janela:** captura/ativação via ferramenta Windows falhou repetidamente com `foreground window did not report a process id`. A inspeção passou a usar capturas do próprio jogo. Não foram simulados cliques nem invocados callbacks de botões para apresentar navegação como validada.
7. **Leitura incompleta de componentes:** UnityPy encontrou falhas de interpretação em 2.124 MonoBehaviours do arquivo principal. Os totais de tipos e muitos recursos visuais puderam ser listados, mas não foi reconstruído todo o grafo de comportamento.

As capturas são evidências locais de execução; contêm pixels proprietários do jogo e não integram um pacote de distribuição.

## Verificação solicitada

| Pergunta | Situação | Evidência e limite |
|---|---|---|
| 1. Os recursos podem ser identificados? | **Sim, parcialmente** | Inventário de todos os bundles, identificação de UI/estágios/atlases e alvo confirmado em execução. Grafo completo ainda não validado |
| 2. O jogo carrega com mod loader? | **Sim, para o teste** | BepInEx IL2CPP build 790 concluiu chainloader e carregou o plugin no menu. Há incompatibilidades específicas de API |
| 3. A substituição visual funciona? | **Sim, neste alvo** | Fundo autoral mostrado em captura; mesma instância registrada no log. Não é confirmação de conversão sem artefatos |
| 4. A navegação continua funcionando? | **Não verificado** | Menu renderizado não equivale a navegação testada. Falha de automação e rótulos ausentes impedem uma conclusão positiva |
| 5. Estados e regras são mantidos? | **Não verificado em gameplay** | Código e arquivos de regras não foram modificados; não houve partida, progressão, load/save ou comparação de estados de sessão |
| 6. O original pode ser restaurado facilmente? | **Sim para o recurso escolhido** | Desativar o marcador reatribuiu o sprite original. Reiniciar com o teste desativado também não requer reparar bundles. A anomalia dos rótulos permanece separadamente não resolvida |
| 7. A técnica permite personagens e animações? | **Plausível, não executado** | Há acesso a SpriteRenderer, bibliotecas e clipes; substituição contínua pode ser sobrescrita por animações ou carregamentos. Necessita outro ensaio específico |

Nenhum arquivo foi encontrado na pasta persistente esperada após o encerramento. Isso é evidência limitada, não uma garantia de integridade de todos os mecanismos de save. Não se comprovou preservação de uma campanha existente.

## Custo e comparação com reconstrução independente

Não há base para uma estimativa confiável em dias de toda a conversão. As contagens incluem duplicatas e o inventário de arte autoral ainda não foi definido. A tabela separa os principais custos qualitativos.

| Trabalho | Mod visual mantendo as regras | Projeto independente |
|---|---|---|
| Um fundo estático | Baixo após resolver carregamento de imagem; viabilidade demonstrada | Baixo, com acesso direto ao projeto |
| UI inteira | Médio: componentes diferentes, atlases, localização e testes de navegação | Médio: recriar telas e fluxos, com liberdade de layout |
| Personagens e expressões | Alto: reproduzir encaixe de partes, variações, máscaras e todas as combinações exigidas pelo jogo | Alto custo de arte, mas estrutura e quantidade de variantes podem ser escolhidas |
| Animações de exercícios | Alto e ainda incerto: respeitar bindings e transições existentes pode restringir poses novas | Alto custo de produção, com rigs, poses e transições adequados à temática |
| Regras, progressão e saves | Menor implementação inicial se a experiência original for preservada integralmente; validação continua necessária | Maior implementação inicial, com controle integral e testes próprios |
| Manutenção | Dependência da versão do jogo, do loader e das APIs de interop | Manutenção sob controle do projeto |
| Entrega autônoma | Um patch/plugin precisa da instalação legítima e não pode incluir os recursos proprietários | Permite produto independente com código e assets próprios |

**Inferência:** para uma troca de aparência limitada e fiel aos fluxos existentes, o mod tem potencial de economizar bastante trabalho de sistemas. O ensaio comprova essa vantagem somente para um fundo. Para transformar poses, atividades e apresentação em uma experiência coerente de academia, a produção de arte e animação continua grande; adaptar-se às dependências existentes pode consumir a economia obtida ao reutilizar a lógica.

Se o objetivo futuro incluir novas regras de treino, progressão diferente ou um produto independente distribuível, a reconstrução fica mais alinhada a esse objetivo. Este relatório não autoriza reutilizar código, cenas ou arte proprietária nesse projeto.

## Recomendação final: C

Realizar **um ensaio A/B de navegação e renderização do menu**, limitado ao mesmo fundo e a telas sem progressão:

1. Comparar cópia sem loader, cópia com loader/plugin desativado e cópia com a troca ativa.
2. Usar um perfil de teste realmente isolado para dados persistentes e preferências.
3. Capturar frames ao fim da renderização e executar `Menu → Créditos → Voltar`, verificando também os rótulos SETTINGS e SAVEFILE sem abrir operações de save.
4. Repetir a restauração e conferir estabilidade dos rótulos, cliques, logs e dados do perfil.

Critério de aprovação: navegação observada nos três casos, fundo substituído/restaurado e ausência de diferenças de UI inexplicadas. Se a anomalia persistir somente com loader/plugin, registrar a diferença antes de investir em personagens. Mesmo um resultado positivo não substitui um ensaio posterior de animação e estados de gameplay.

**Esse próximo experimento não foi iniciado.** O marcador de substituição ficou desativado, o recurso foi restaurado e os processos do jogo e do AssetRipper usados na investigação foram encerrados. As ferramentas e evidências locais foram preservadas apenas na pasta de pesquisa.
