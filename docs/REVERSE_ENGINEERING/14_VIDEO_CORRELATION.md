# Correlação com vídeos — pendente de revisão visual

## Atualização de status — fase 3

O impedimento histórico abaixo foi superado: a fase 3 usou Windows WPF MediaPlayer local e examinou 55 amostras dos cinco arquivos. A linha do tempo vigente está em 18_REFERENCE_TIMELINE. Não foi contornado o bloqueio do navegador; foi usado um decodificador local sob o novo escopo autorizado.

## Registro anterior preservado

23/10/2026. **UNKNOWN:** não foram observados acontecimentos visuais nem estabelecidos timestamps de gameplay nesta etapa. Nenhuma hipótese de código foi promovida a confirmação por vídeo.

## Evidência disponível e impedimento

**CONFIRMED:** os cinco MP4 existem em `References/GameplayVideos`. Foram lidos somente tamanho, duração e timescale de seus contêineres; registro privado em `StaticPhase2/video_metadata.json`.

| Arquivo | Duração do contêiner | Revisão visual | Timestamps de acontecimentos |
|---|---:|---|---|
| 04_GameplayRun.mp4 | 135,933 s | Não realizada | **UNKNOWN** |
| 02_SessionStart.mp4 | 13,466 s | Não realizada | **UNKNOWN** |
| 05_UpgradeStore.mp4 | 19,300 s | Não realizada | **UNKNOWN** |
| 03_Galery.mp4 | 43,966 s | Não realizada | **UNKNOWN** |
| 01_MainMenu.mp4 | 22,733 s | Não realizada | **UNKNOWN** |

Foi tentada uma abertura de 04_GameplayRun no navegador integrado, oculto. A política do navegador rejeitou o protocolo `file:`; apenas http/https eram permitidos. Não se tentou servidor intermediário, outra superfície de navegador ou outro contorno para essa rejeição. A busca prévia por decodificadores já instalados não encontrou ffmpeg/ffprobe, cv2 ou av nos locais consultados. Isso descreve a disponibilidade verificada, não prova que não exista nenhum player no computador.

Nenhum programa do jogo foi iniciado. Não foram salvos frames, thumbnails, diálogos, áudio extraído ou material visual proprietário. Duração do arquivo não é evidência de quando um evento acontece.

## Roteiro curto para correlação posterior

A tabela é uma lista de observações a procurar, **INFERRED como associação candidata**, não um registro de ações que já ocorreram no vídeo. Preencher timestamps somente após observar o arquivo em um player permitido.

| Acontecimento a localizar | Sistema candidato | Evidência nativa que permite comparar | Timestamp / confiança atual |
|---|---|---|---|
| Apresentação do elenco e escolha | Roster.ShowAndAWaitResponse / ChooseClient | Espera por ID e remoção dos disponíveis | Não observado / **UNKNOWN** |
| Entrada de personagem e diálogo | MakeClientVisit / WorldStage | Entrada precede diálogo e espera da mão | Não observado / **UNKNOWN** |
| Mão de habilidades aparece | SkillMenu / PrepareHand / UISkillMenu | Compra padrão e apresentação separadas | Não observado / **UNKNOWN** |
| Arraste retorna ou confirma | UISkill.HandleOnPointerUp | Limiar de altura/3 no caminho de ponteiro | Não observado / **UNKNOWN** |
| Atividade começa e termina | SkillPlayer / UIActivity / ActivityPlayer | Sequência de awaits recuperada | Não observado / **UNKNOWN** |
| Nota aparece e comando continuar | UICumplete / DetectNext | Score é registrado antes da espera de continuar | Não observado / **UNKNOWN** |
| Saída do cliente e próxima escolha | MakeClientVisit / StageTurnPlayer | Limpeza e fim da visita | Não observado / **UNKNOWN** |
| Fim do dia / resultados / upgrades | StageDayPlayer / LevelFlow | Dia intermediário segue para resultados e upgrades | Não observado / **UNKNOWN** |

Para cada observação futura, registrar timestamp inicial/final, ação visível sem transcrever diálogos, sistema candidato e grau de confiança. Um vídeo pode corroborar sequência e apresentação; não demonstra os pesos internos de preferência, a fórmula de stamina nem a totalidade das regras de progressão.
