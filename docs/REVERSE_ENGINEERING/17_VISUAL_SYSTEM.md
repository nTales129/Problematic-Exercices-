# Sistema visual — amostra representativa

23/10/2026. Fontes P3-N/P3-V definidas no documento 16. Foram lidos metadados e uma amostra de dados dos bundles de dois estágios, sem reconstruir prefabs ou exportar sprites/controladores para o projeto autoral.

## Composição e divisão de responsabilidades

**CONFIRMED:** uma atividade é uma apresentação composta: PlayableActivity contém configuração de ID, localização, área e referência de diálogo; ActivityPose agrega Animator, SpriteRenderers, personagens de pose e addons. As listas de componentes não precisam estar todas serializadas: `ActivityPose.Initialize`, `1892a90`, consulta GetComponent/GetComponentsInChildren e grava os campos do Animator e coleções, conforme offsets originais.

**CONFIRMED na amostra serializada:** dois ActivityPose contêm somente campos de MonoBehaviour no typetree, mas os GameObjects associados têm SpriteRenderer, Animator, Transform e outros componentes. Ausência de arrays no asset não significa ausência dos personagens em runtime. O código reúne parte dessa composição pela hierarquia.

| Responsabilidade | Código recuperado | Dados/recursos recuperados | Limite |
|---|---|---|---|
| Selecionar apresentação | Habilidade entrega ActivityId; ActivityPlayer resolve a apresentação | ActivityPrefabLibrary contém coleção de PlayableActivity | **CONFIRMED**; biblioteca de amostra tem 17 referências, incluindo a atividade examinada |
| Identidade visual intercambiável | SwapCharacter escolhe biblioteca atual e SpriteBy(characterId); escreve SpriteRenderer.sprite | FaceSwappable referencia renderer, biblioteca padrão, biblioteca de conclusão e anchor opcional | **CONFIRMED** em `1891440`, `1891370` e amostra |
| Personagem fixo | Interface de ID, Transform e IsActive | FixedCharacter possui characterId e headTransform | **CONFIRMED**; não exige troca de rosto |
| Ritmo da pose | ChangeSpeed escreve Animator.speed | Clips e controllers associados aos GameObjects | **CONFIRMED**, `18928c0`; não altera pontuação por si só |
| Conclusão visual | Troca biblioteca da variante e dispara Animator.SetTrigger em `18929f3` | Controller possui referências a clips e parâmetro de conclusão | **CONFIRMED** na amostra; predicados de todos os controllers não reconstruídos |
| Posição e sobreposição | Setup ativa GameObject e configura personagens; WorldStage coordena apresentação | Transform local/parent, SortingGroup, sortingOrder, anchors | **CONFIRMED**; coordenadas globais finais de todas as cenas **UNKNOWN** |
| Reset | Zera identidade/biblioteca/sprite de componente intercambiável; reseta velocidade e componentes | Estado inicial dos prefabs/Animator | **CONFIRMED** no caminho normal; recuperação integral de exceções **UNKNOWN** |

## Bibliotecas, identidade e expressões

**CONFIRMED:** CharacterSpriteVariationLibrary associa CharacterId a Sprite; a amostra contém 17 associações com referências externas. Esse mapeamento fornece identidade/variação dentro de uma pose. Ele não prova que toda emoção seja selecionada por essa mesma biblioteca.

**CONFIRMED:** ActivityFaceSwappableCharacter.SwapCharacter usa a biblioteca padrão se a atual não está disponível; o caminho de conclusão troca para outra biblioteca. Reset limpa biblioteca, ID e sprite (`1891400`). A variante fixa pode compor outras partes da mesma apresentação. Na amostra, headTransform pode ser nulo: não é um anchor obrigatório em todos os componentes.

**CONFIRMED em dados:** MainCharacterAnimationsConfig possui grupos, flag isRandom, intervalos de tempo, referências a clips e máscaras usedInLevels com valores 1/2/4/8. Existem caminhos Idle e PayingAttention no código. **STRONGLY INFERRED:** grupos e níveis escolhem conjuntos de reação contextual. **UNKNOWN:** peso/probabilidade, correspondência de todas as expressões e resolução completa dos bitmasks; não se conclui que toda reação seja uma troca simples de rosto.

## Poses, clips e controllers

**CONFIRMED:** os clips examinados incluem bindings para Transform (typeID 4) e SpriteRenderer (212), além de mapeamento de referências de sprites. Alguns arrays editoriais de curvas estão vazios enquanto os bindings e dados compilados existem. Não interpretar isso como animação vazia.

**CONFIRMED na amostra principal:** dois clips usam sample rate 13 e loop, com intervalos aproximados 0–0,462 s e 0–0,385 s. Esses loops curtos não determinam a duração da visita: a apresentação pode repetir enquanto o fluxo aguarda interação e diálogo. Não confundir sample rate de clip com os 30 fps dos vídeos.

**CONFIRMED:** um controller de efeito tem uma camada, uma máquina e um clip; outro de atividade tem uma camada, uma máquina, dois clips e um parâmetro. Uma ligação direta foi resolvida do Animator de um personagem fixo para esse controller. Outro estágio possui um controller mais complexo com 15 referências de clips; isso delimita a diversidade, sem exigir sua reconstrução.

**CONFIRMED:** PlayableActivity.SetUp (`1899100`) ativa a raiz, guarda ID do cliente, anexa o timespan e percorre personagens/addons. HandleMultiplierChanged (`18985c0`) atualiza velocidade dos animators das poses. O ID de atividade e a configuração selecionam apresentação; os recursos descrevem a execução visual e a hierarquia. Não foi encontrado motivo para colocar a fórmula de score dentro de um AnimatorController autoral.

## Amostra rastreável

Chave completa inclui bundle, arquivo serializado, PathID e FileID das referências. O mesmo PathID pode aparecer em outro bundle.

| Elemento | Bundle abreviado | PathID |
|---|---|---:|
| PlayableActivity examinado | 9c1d36678f140b452c6721d68c96bb31 | -8671181009404565606 |
| Biblioteca que o referencia | mesmo bundle | -956106089238084510 |
| ActivityPose | mesmo bundle | -7701503058448869820 |
| Personagem intercambiável | mesmo bundle | -6761543141613014647 |
| Personagem fixo | mesmo bundle | -8530961763295261129 |
| Biblioteca de variação | mesmo bundle | 3841468536879167147 |
| Grupos de animação principal | mesmo bundle | 6311986556197231017 |
| Controller ligado ao personagem fixo | mesmo bundle | -7992361830803959809 |

O arquivo serializado principal dessa amostra é `CAB-30e100551230572b7a1cc8ebf909a187`. Dados completos das referências e resumos ficam apenas em visual_sample.json/visual_links.json. Algumas bibliotecas de rosto referenciadas externamente não são a biblioteca de variação amostrada; não foi inventada uma ligação entre elas.

## Aplicação independente

**ORIGINAL DESIGN DECISION:** usar definições autorais de atividade e atores, um presenter que instancia composição própria e um resultado de domínio independente. Cada ator pode ter corpo, expressão e anchor de balão opcionais. Começar com formas geométricas e dois estados de animação autorais, depois substituir a apresentação sem reescrever deck/score.

**ORIGINAL DESIGN DECISION:** identificar no novo projeto estados semânticos como entrada, idle, exercício, reação e saída, com fallback visual quando faltar expressão. IDs e parâmetros dos controllers serão próprios. Falta de um recurso visual deve mostrar placeholder e permitir conclusão controlada; configuração de domínio inválida deve impedir início com erro legível. Nenhum asset ou código dessa amostra será um recurso de produção.
