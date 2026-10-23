# Problematic Exercise

Jogo independente de academia em Unity **6000.6.6f1**, com código, personagens, textos e placeholders próprios. Busca alta fidelidade funcional às regras investigadas, sem incorporar código ou recursos do jogo de referência.

Abra esta pasta no Unity Hub, aguarde a restauração dos pacotes e abra `Assets/ProblematicExercise/Scenes/Campaign.unity`. Pressione Play. O menu **Problematic Exercise → Open Campaign** também prepara/abre a cena.

Campanha de cinco dias com cotas3/4/5/6/6. Selecione cliente, aguarde entrada/fala, escolha uma carta e confirme pelo arraste ou pelo botão. Conclua a atividade, aguarde a reação e continue. O resultado diário permite avançar; o resultado final permite voltar ao menu. Fechar descarta o estado da sessão; save não está implementado.

Consulte [handoff](docs/HANDOFF.md), [especificação](docs/DESIGN/GAMEPLAY_SPEC.md), [matriz de fidelidade](docs/DESIGN/FIDELITY_MATRIX.md) e [relatório da Fase6](docs/DEVELOPMENT/PHASE6_CAMPAIGN.md). As cenas FirstVisit/FirstDay são fixtures de regressão.

Testes: Unity Test Runner, EditMode e PlayMode, namespace `ProblematicExercise.Tests`. Build Windows: definir `PE_CAMPAIGN_BUILD` para uma pasta de saída privada e executar `ProblematicExercise.Editor.CampaignTools.BuildWindows` em batch. Não incluir builds, Library, referências audiovisuais ou pesquisa privada no Git.

Recursos TextMesh Pro/Unity e Liberation Sans conservam suas licenças e avisos incluídos. Galeria, loja, save, melhorias, recompra e arte definitiva não estão concluídos.
