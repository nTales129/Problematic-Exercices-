# Dia, recursos e progressão

## Atualização de status — fase 3

A fase 3 identificou level>0 no filtro diário, seleção obrigatória de tutorial e visita direta por evento. Os vídeos corroboram resultado diário, recompensa e novo dia. Economia autoral e decisões que substituem lacunas estão em GAMEPLAY_SPEC; não são novos achados do original.

## Registro anterior preservado

23/10/2026. Análise estática, sem execução do jogo. Os endereços são RVAs da build examinada. Métodos e artefatos rastreáveis no documento 13. As fórmulas abaixo têm domínio indicado; não pressupõem validade de configurações arbitrárias ou ausência de overflow em entradas malformadas.

## Stamina e consumo

**CONFIRMED — metadados originais:** `Stamina` mantém máximo inteiro, valor atual inteiro, modificador de custo float, flag de infinito e evento de esgotamento. `IsOutOfStamina` compara o valor atual com zero (`18ee000`). O máximo e o valor atual não são o mesmo campo.

**CONFIRMED — `Stamina.Change`, `18edbc0`:** quando não está infinita e a mudança é permitida, o novo valor é limitado ao intervalo [0, máximo]. Se o valor já é zero, a mudança comum retorna sem alterá-lo; o parâmetro de revive permite outro caminho. Ao atingir zero, invoca o callback de esgotamento e emite mensagem de KO; com resultado positivo, emite atualização de valor. Isso não é uma simples subtração sem saturação.

**CONFIRMED:** `SkillStaminaCostHelper.SkillStaminaCost` (`18ea380`) consulta a raridade e retorna 10, 25, 40 ou 55 para seus quatro valores válidos 0–3; outro valor segue exceção. A associação à raridade é sustentada pela posição do método na interface e pelos contratos originais, não pelos números isolados.

**CONFIRMED nas operações nativas:** `ApplyCostReduction` subtrai `percentual / 100f` do modificador (`18edba0`; constante 100 lida do arquivo). `Decrease` nega o custo e, no caminho sem penalty, multiplica pelo modificador antes de uma rotina de arredondamento/conversão e `Change` (`18ede00`). O handler de habilidade normal faz operações equivalentes (`18ea440`).

**CONFIRMED após seguir o auxiliar:** o corpo em `3d3868–3d3917` remove a parte fracionária por máscara e, para negativo não inteiro, subtrai 1; para -1<x<0 retorna -1, para 0<x<1 retorna 0. As constantes -1 e +1 foram lidas do PE. Portanto implementa floor no domínio finito normal; o nome original da função não foi identificado. Não é arredondamento para o mais próximo.

Para custo-base c≥0, modificador finito m≥0, sem penalty, infinito ou bloqueio de KO, e conversão dentro do intervalo int32:

```text
q = float32((-c) × m)
delta = int32(floor(double(q)))
stamina nova = clamp(stamina atual + delta, 0, máximo)
```

**CONFIRMED:** a multiplicação ocorre em float32 antes da conversão para double. No domínio positivo habitual, o gasto corresponde a ceil do produto positivo já representado em float32. Exemplo sintético: c=25 e m=0,9 produzem delta=-23, gasto 23; c=10 e m=0,85 produzem delta=-9. Não houve execução do helper ou do jogo: os exemplos derivam das operações recuperadas. **UNKNOWN:** política global para modificadores negativos/excessivos, não finitos e overflow; todos os upgrades que os alteram. O caminho penalty pula essa multiplicação/arredondamento e passa o custo negado diretamente a Change.

**STRONGLY INFERRED:** o sinal de uso no início de PlaySkill aciona os listeners registrados pelo addon de stamina. **CONFIRMED:** o handler comum aplica o custo à stamina do jogador; o handler de Gym (`1918600`) percorre os IDs de personagens da habilidade e chama Decrease nas respectivas instâncias de stamina. Não há suporte para tratar os dois estágios como um único contador universal.

**CONFIRMED:** o handler de seleção possui caminhos de simulação de mudança, distintos do handler de habilidade jogada. A prévia não deve ser contada como consumo real. **UNKNOWN:** todos os gatilhos de aumento, revive, KO especial e a composição completa de fontes de KO por estágio.

## Por que o dia termina

**CONFIRMED:** `EarlyEndChecker.CheckForEarlyEnd` (`18c4ea0`) encaminha para `Player.IsKO`. Este consulta a coleção `koSources` (`18e6110`), não somente um campo fixo de stamina. A estrutura é compatível com múltiplas fontes; a operação genérica de agregação e o conjunto completo de fontes precisam de rastreamento adicional.

**CONFIRMED:** `StageTurnPlayer.Play.MoveNext` verifica KO entre etapas quando `continuesEvenIfEarlyEnd=false`; sempre volta a verificar ao produzir o resultado final do turno. O caminho final com KO retorna `endsDay=true` e `EndDayReason.OutOfStamina=1`; sem KO, `endsDay=false`. A flag true pula somente a verificação intermediária. O documento 11 detalha essa condição.

**CONFIRMED — `StageDayPlayer.PlayDay.MoveNext`, `18d3f50`:**

1. Atualiza spawns a partir das habilidades do jogador e configura o roster com `Day.ClientsInDay`.
2. Configura dados diários do diretor, apresenta aviso de início e muda para StartOfDay.
3. Processa a fila General de eventos.
4. Enquanto houver clientes disponíveis **ou** turnos extras, obtém um turno e aguarda StageTurnPlayer.
5. Se o resultado do turno tem endsDay, sai com motivo de stamina; caso contrário, repete a verificação do provedor.
6. Provedor esgotado encerra com OutOfClients=0. Apresenta aviso correspondente, muda para EndOfDay, processa eventos EndOfDay=2 e calcula/aplica o resultado diário.

Evidências: `18d4179`, `18d4453`, `18d4503–18d451d`, `18d45ef–18d4643`, `18d4776`, `18d48c9–18d48e3`, `18d4b41–18d4c0f`.

**CONFIRMED, precedência local:** após uma visita que simultaneamente esgota o elenco e causa KO, o resultado do turno com endsDay sai antes de nova consulta ao provedor; nesse caminho o aviso é de stamina. Sem turno pendente, o loop termina por esgotamento. Isso não prova prioridade universal sobre todos os eventos especiais: o diretor também verifica KO e pode descartar eventos não importantes.

**CONFIRMED:** DayCleanUp chama limpeza do jogador, desativa personagens principais e limpa turnos extras (`18b5f20`). DaySetUp reseta UI, prepara Player e ativa personagens (`18b6070`). Player.DaySetUp limpa scores diários, reseta o gerenciador de habilidades e reaplica efeitos passivos (`18e5940`). Os addons também recebem chamadas de ciclo diário. **UNKNOWN:** garantia de limpeza equivalente em todo caminho excepcional/cancelado.

## Resultados e fórmulas

Considere os scores do dia, o valor em pontos V(g) da nota definido no documento 03, o custo P>0 de um presente, o número extra E, o saldo C de pontos de presentes, o progresso acumulado T e seu máximo M>0.

**CONFIRMED — redução dos scores:** `CalculateTrueEndingProgress` (`19159e0`) e `CalculatePresentsGainedInPoints` (`19158a0`) usam a mesma redução genérica sobre obtainedScores. O corpo em `6ee4e0` recebe acumulador inicial zero, percorre o enumerador, invoca o delegate com acumulador e elemento e guarda o retorno para a próxima iteração (`6ee621–6ee63b`). Os dois delegates compartilham `19279a0`: chamam GradeValue e somam o acumulador em `19279b7`. Assim:

```text
S = soma de V(nota) dos scores do dia
ganho de progresso = S
ganho de pontos de presentes = S + P × E
```

A soma é **CONFIRMED** pelo laço e pelo delegate, mesmo sem recuperar o nome exato da especialização genérica. O callback soma inteiros sem branch de overflow nesse trecho; as fórmulas usam entradas cujo total cabe em int32. Não foi atribuído à redução um comportamento de soma verificada por exceção.

**CONFIRMED — aplicação e apresentação:** o construtor de EndOfDayResult (`1915b10`) calcula `prevLeftOverPresents = float(C)/float(P)` e `presentsToAnimate = float(ganho)/float(P)`. Esses valores são floats de apresentação; não há divisão inteira ou resto nesse trecho. `Player.CalculateAndApplyEndOfDayResult` (`18e5240`) acrescenta o ganho ao saldo de pontos e limita `T+ganhoDeProgresso` a [0,M].

**CONFIRMED — progresso:** `TrueEndingProgress` limita o progresso ao máximo, calcula normalização por divisão float e testa máximo atingido. O construtor diário consulta valores anterior, tentativo e máximo atingido para `DayProgressResult`. A relação matemática é:

```text
Tnovo = clamp(T + ganhoDeProgresso, 0, M)
normalizado anterior = clamp(T, 0, M) / M
normalizado tentativo = Tnovo / M
atingiu máximo tentativo = (Tnovo == M)
```

Evidências: aplicação `18e5353–18e537a`; normalização/tentativa `18ee7d0`, `18ee830`, `18ee860`. As divisões não protegem P=0 ou M=0 nesses trechos; domínio positivo é uma precondição da fórmula, não uma validação global demonstrada.

**UNKNOWN:** momento exato de converter saldo em aberturas de presentes, regras completas de escolha/recompensa, desconto do saldo após abertura e overflow da redução. Não confundir `presentsToAnimate` com quantidade inteira já concedida. Os parâmetros serializados da etapa 1 passam a ter utilização localizada, mas não permitem reconstruir a economia inteira.

**CONFIRMED como consequência matemática — exemplo sintético, sem execução:** três notas C/B/W produzem S=14+18+34=66. Com P=35 e E=1, ganham-se 101 pontos de presentes, ou aproximadamente 2,8857 unidades para animação; não se afirma que o jogo abra 2 presentes nesse momento. Se T=470 e M=500, o progresso aplicado satura em 500. O exemplo não utiliza cliente ou texto do jogo e não foi executado no runtime.

## Avanço de dias e final

**CONFIRMED:** DayProvider começa com índice -1 (`18ada70`). NextDay incrementa o índice, lê a configuração correspondente, produz número público índice+1 e marca último dia quando o índice chega a comprimento-1 (`18ad9b0`). Não é um ciclo infinito nem uma repetição automática da última configuração; acesso fora do array tem caminho de erro.

**CONFIRMED — `StagePlayerScreen.LevelFlow.MoveNext`, `1909220`:** chama NextDay e DaySetUp, toca música principal, aguarda PlayDay. Testa `Day.isLastDay` no retorno (`1909371`). Nos dias intermediários, apresenta resultado diário, limpa o dia, apresenta UpgradeSection e aguarda sua saída; após reset dessa seção volta ao ciclo (`190a4cf → 1909689`). No último dia, calcula resultado final, cria ending e mostra UIEndOfRun. Isso não equivale a ter analisado custos e conteúdos da loja.

**CONFIRMED — `Player.CalculateEndOfRunResult`, `18e53a0`:** calcula fração progresso/máximo. Sem override de ending, compara progresso com máximo: menor obtém failed ending; maior ou igual obtém true ending. Com override, usa o ID gravado no jogador. Depois monta resultado com medalhas. **UNKNOWN:** fórmula completa de medalhas, todos os produtores de override e condições de desbloqueio permanente.

**CONFIRMED:** LevelFlow contém chamadas de criação de dados de save antes de avanço em certas condições. Elas foram somente lidas como instruções; nenhum save foi aberto ou produzido. **UNKNOWN:** persistência completa entre versões/sessões e todas as condições de continuação após carregamento.

## Clientes, visitantes e eventos

**CONFIRMED:** quantidade diária vem de Day, derivado do array de configurações. Roster tem flag de aleatoriedade por dia, lista disponível e lógica substituível. SetUp consulta a lógica, substitui o elenco disponível e embaralha (`18ab930`). A escolha remove o escolhido do conjunto disponível, evitando sua nova escolha pelo mesmo caminho sem reposição externa.

**STRONGLY INFERRED:** `ObtainClientsForDay` (`18ab1b0`) distingue modo aleatório, aplica uma filtragem e completa quantidade a partir do restante com Except. **UNKNOWN:** significado completo do primeiro filtro, garantias de repetição entre dias, quantidade insuficiente em todos os estágios e ponderações. Não se apresenta distribuição uniforme sem analisar também os auxiliares de embaralhamento.

**CONFIRMED:** visitantes possuem biblioteca/configuração separada do roster de clientes. O diretor mantém filas por General, SkillPlayEnded e EndOfDay e resolve players de eventos; `PlayQueuedEvents.MoveNext` (`18d6500`) verifica KO e remove eventos não importantes (`18d6793–18d67bf`). Retorna o estado de KO ao finalizar (`18d6ee9`), não simplesmente “houve evento”.

**STRONGLY INFERRED:** eventos podem intercalar apresentações e alterar o fluxo sem representar um cliente escolhido do elenco. **UNKNOWN:** mapa completo de gatilhos, visitantes que criam turnos extras, efeitos narrativos sobre recursos e todas as interrupções. Não foi feita extração de textos ou catálogo integral dos eventos. Esta parte do bloco C permanece parcial.

**ORIGINAL DESIGN DECISION:** para o jogo de academia, separar dias, visitas, recursos, resultados e eventos é uma opção de arquitetura sustentada pela investigação; os valores e regras autorais precisam ser definidos em especificação própria. Nada foi implementado nesta etapa.
