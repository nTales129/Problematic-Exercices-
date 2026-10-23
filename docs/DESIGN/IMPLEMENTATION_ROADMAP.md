# Roadmap de fidelidade — v0.2

## Entrega autorizada na fase 5

Primeiro atendimento aceito manualmente pelo usuário. Agora implementar somente menu, dia1 com até três visitas, continuidade de roster/deck/stamina/score, encerramento e resultado diário. Dados das cinco cotas preparados, sem dias futuros. Tutorial, recompra, demais fontes de KO, eventos, recompensas, campanha, galeria, loja, save e arte final seguem adiados. Essa autorização substitui o estado “etapa1 agora” da tabela histórica abaixo.

Validação atual: 34 EditMode e cinco PlayMode aprovados (os 28 anteriores preservados). Percurso completo do dia na janela Windows concluído por cliques reais; arraste manual e fechamento por Alt+F4 têm limitações registradas em `docs/DEVELOPMENT/PHASE5_FIRST_DAY.md`. Não inferir navegação do executável somente pelo Editor.

23/10/2026. Usuário autoriza etapa1 agora. Restante é planejamento, não autorização para ampliar entrega. Consultar spec/matriz v0.2.

| Etapa | Objetivo / arquivos | Dependência | Aceitação | Teste Unity |
|---|---|---|---|---|
| 1 atual | Domain, Runtime, cena FirstVisit, placeholders | Spec/matriz corrigidas | Início→cliente→entrada→3cartas→prévia→confirmar→atividade→nota→resultado→saída;AC01–13 | EditMode, Mouse/EventSystem PlayMode, Console/build se disponíveis |
| 2 | Dias/roster/tutorial | Revisão fatia e novo escopo | Perfil Detention sem misturar Gym | Filtro/opções/remoção/override |
| 3 | Requisitos/recompra | Resolver inicial/reset necessários | Carga antes compra, sem emergência | Reciclagem/mão curta/zero cargas |
| 4 | Encerramento diário/KO | Dias/roster | Última visita completa, precedência local | Esgotamento simultâneo |
| 5 | Progresso/melhorias | Dias e lacunas dirigidas | S,500,P35,E0,sem economia inventada | Soma/clamp/fluxo final |
| 6 | Eventos | Visita/dias e novo escopo | Filas/flags/players recuperados | Interrupção/KO/retorno |
| 7 | Save independente | Modelo estável | Checkpoints próprios explícitos | IO temporário/carga |
| 8 | Galeria/loja | Esclarecer UNKNOWN relevantes | Conteúdo próprio/regras explícitas | Replay/compra/navegação |
| 9 | Arte/áudio | Aprovação artística | Trocar placeholders sem mudar regras | Regressão afetada |

Não retomar modding/A-B-C, executar original, usar PE_Tester, editar WorkingCopy/Gym Discipline ou fazer commit/push. Ver FIRST_PLAYABLE_VALIDATION para execução real e limites.
