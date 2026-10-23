# Linha do tempo das referências locais

23/10/2026. **OBSERVED** significa quadro efetivamente examinado de uma gravação, não uma execução realizada pelo agente. Os sistemas candidatos são sustentados separadamente por código; associação visual não prova fórmula interna.

## Método e precisão

FFmpeg/FFprobe executáveis não foram encontrados nos locais consultados. As DLLs ffmpeg de aplicativos Electron não são ferramentas CLI. Uma tentativa de obter imageio-ffmpeg falhou por resolução DNS de pypi.org; não houve instalação nem insistência. O fallback local usou **Windows WPF MediaPlayer**, mudo e sem janela, com leitura de arquivos e RenderTargetBitmap. Não houve navegador, jogo original, loader ou conta PE_Tester.

O pedido da fase 3 autoriza capturas representativas fora de Assets e do repositório autoral. Uma rejeição automática inicial aplicou a restrição anterior; após conferir a autorização nova, a captura limitada foi permitida. Os PNG e sequências de pesquisa ficaram exclusivamente em `ProblematicSubjects_Research/StaticPhase3/Captures`. Nenhuma imagem foi incorporada aos documentos ou assets de produção.

**CONFIRMED por contêiner:** todos os vídeos têm 1280×720, 30 fps constantes no track de vídeo. A contagem e a duração abaixo vêm de stts/mdhd/tkhd, não da duração arredondada do player.

| Arquivo | Duração | Frames no arquivo | Quadros amostrados |
|---|---:|---:|---:|
| 04_GameplayRun | 135,933 s | 4.078 | 32 |
| 02_SessionStart | 13,467 s | 404 | 5 |
| 05_UpgradeStore | 19,300 s | 579 | 6 |
| 03_Galery | 43,967 s | 1.319 | 6 |
| 01_MainMenu | 22,733 s | 682 | 6 |

Foram 55 amostras direcionadas, não uma exportação sequencial dos 7.062 frames. Logs registram posição solicitada e retornada; houve exemplo de seek a 50 s reportando 50,01 s. O backend não certifica PTS do quadro renderizado: timestamps abaixo são **pontos de amostragem aproximados**, e intervalos delimitam mudanças vistas. Não foram medidos instante exato de clique, duração contínua de todas as animações ou latência de input.

## 04_GameplayRun — sequência principal

Todos os acontecimentos visuais da segunda coluna são **OBSERVED**. A correspondência com sistema específico é **STRONGLY INFERRED**, salvo onde indicada como apenas candidata.

| ID / tempo aproximado | Acontecimento observado, sem transcrição de diálogo | Sistema candidato / evidência |
|---|---|---|
| V01 — 00:00 | Seleção de estágio, botão de jogar e galeria | StageSelectSection; referência começa antes da sessão |
| V02 — 00:08 | Cenário principal com ator em idle e barras superiores | StageDayPlayer/WorldStage; início efetivo ocorre entre amostras 0–8 |
| V03 — 00:20–00:30 | Aviso de mensagem, escolhas de resposta e indicação de chegada; painel desaparece | MailEventPlayer/eventos; associação a configuração exata **INFERRED** |
| V04 — 00:35 | Cliente presente à esquerda e ator principal à direita | EnterVisitingClient concluído; não prova seu instante inicial |
| V05 — 00:38–00:39 | Três cartas na parte inferior, diálogo e balão de tag na carta destacada | SkillMenu/UISkill/preview; selecionar não executa imediatamente |
| V06 — 00:39,5 | Uma carta aparece elevada, separada da fileira | Arraste observado; não foi o agente quem arrastou |
| V07 — 00:40–00:40,5 | Carta e personagens atravessam fade; a composição de atividade substitui a anterior | Confirmação e transição para SkillPlayer/ActivityPlayer; instante do release não visível como evento de input |
| V08 — 00:42 | Atividade e diálogo ativos | PlayableActivity/diálogo; corresponde ao fluxo nativo de apresentação |
| V09 — 00:46 | Nota C em destaque sobre a atividade, com reação facial | Apresentação de resultado; pesos internos não dedutíveis desse quadro |
| V10 — 00:50 | Cenário retorna ao idle, nota C acumulada no HUD e duas opções de retrato | Saída/roster para próxima escolha; não é uma segunda habilidade na mesma visita |
| V11 — 00:54–00:58 | Um retrato permanece com indicação de chegada | Escolha de cliente seguida de chegada |
| V12 — 01:02 | Novo aviso de mensagem aparece durante a espera | Intercalação de evento; não atribuir inserção em extraTurns sem evidência |
| V13 — 01:10 | Outro cliente presente | Nova visita |
| V14 — 01:16 / 01:20 | Outra apresentação de atividade e depois nota C | Segunda sequência observada de atividade/avaliação |
| V15 — 01:24–01:34 | Novo retrato/chegada; mensagem de resposta; HUD mantém CC | Fluxo de próximo cliente com evento intercalado |
| V16 — 01:38 | Cliente presente e três cartas; uma opção aparece indisponível/cinza | Mão e disponibilidade visual; motivo da indisponibilidade **UNKNOWN** pelo vídeo |
| V17 — 01:42 / 01:46 | Atividade em outro posicionamento e retorno ao idle; HUD mostra BCC | Terceira visita e resultado acumulado; ordem visual BCC não prova ordenação de armazenamento |
| V18 — 01:50 | Aviso de fim do dia sobre o cenário | DayNotice/StageDayPlayer |
| V19 — 01:54 | Resultado diário exibe notas, progresso e presente | UIEndOfDayResult; corroborado pela soma/progresso recuperados, sem medir valores por pixels |
| V20 — 01:58 | Tela de plano/upgrades com presente disponível | UpgradeSection; distinta da loja de medalhas de 05 |
| V21 — 02:02 | Modal apresenta duas alternativas e botão de escolha | Recompensa/upgrade de sessão; nenhum clique de compra foi testado pelo agente |
| V22 — 02:06 | Aviso do dia 2 e cenário | Retorno do ciclo de upgrades para próximo dia |
| V23 — 02:10 | Visitante e diálogo no cenário após início do dia | Evento de visitante candidato; não demonstra cliente selecionável nem nota |

**OBSERVED — ritmo amostrado:** a primeira mão visível é seguida de atividade em cerca de 2–3 s entre os pontos examinados; a nota está presente alguns segundos depois; a amostra seguinte mostra o roster. Esses intervalos incluem decisões humanas, diálogos e amostragem. Não são constantes temporais do motor a reproduzir exatamente.

## Outras referências

| Vídeo / tempo | OBSERVED | Relação e limite |
|---|---|---|
| 02 — 00:00 | Menu principal | Ponto de entrada visível |
| 02 — 00:03, 00:06, 00:09 | Seleção alterna entre cartões de estágios, com progresso visível | Seleção antes de jogar; algoritmo de desbloqueio não demonstrado |
| 02 — 00:12 | Quadro preto após seleção | Transição visível; não se afirma que a sessão carregou antes do término |
| 05 — 00:04–00:16 | Loja de medalhas, lista rolável, descrição, itens adquiridos e switches | Economia permanente/ativação de utilidades; sem demonstração suficiente de pagamento/custo de item não adquirido |
| 05 — 00:19 | Retorno à seleção de estágio | Navegação de retorno corroborada |
| 03 — 00:10 | Galeria com cliente, fileira de atividades e acesso a finais | Replay organizado por atividade/ator |
| 03 — 00:20 | Apresentação de uma atividade na galeria | Reutilização visual candidata; não pressupor consumo de recursos |
| 03 — 00:30 | Catálogo de finais | Outra categoria de galeria |
| 03 — 00:40 / 00:43 | Tela de final e retorno ao catálogo | Visualização/retorno observados, sem transcrever conteúdo |
| 01 — 00:00 / 00:22 | Menu com SETTINGS e SAVEFILE legíveis | Rótulos presentes nesta gravação; não valida testes históricos A/B/C |
| 01 — 00:05 | Modal de ajustes, volumes e resolução/framerate | Apresentação observada; não foram alteradas preferências pelo agente |
| 01 — 00:10 | Modal informativo de save | Não é evidência de uma interface completa de slots ou load manual |
| 01 — 00:15 / 00:20 | Créditos e changelog | Navegação por painéis |

## Correções às hipóteses e lacunas

- **OBSERVED:** o fluxo contém mensagens e visitante entre atendimentos; um diagrama que os omitisse seria incompleto.
- **OBSERVED:** a carta é elevada antes da transição, corroborando a separação técnica entre prévia, arraste e confirmação. O vídeo não mede o limiar geométrico; altura/3 vem do código.
- **OBSERVED:** resultado diário, recompensa de sessão e loja permanente têm apresentações distintas. Não modelar tudo como uma mesma compra.
- **UNKNOWN:** fórmula da nota a partir desses quadros, origem de toda reação, custo exato de item comprado, condições de desbloqueio, saves e todos os eventos entre amostras.

As referências orientam sequência e legibilidade. O novo jogo terá conteúdo, identidade visual, layout e tempos autorais; critérios comparáveis e diferenças deliberadas constam de GAMEPLAY_SPEC.
