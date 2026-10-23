# Problematic Exercise — validação controlada de estabilidade

Data: 23/10/2026 — America/Sao_Paulo.

## Atualização — conta PE_Tester e pacote preparado

Esta atualização substitui o bloqueio genérico de ausência de conta descrito nas seções históricas abaixo. **A conta existe; falta entrar nela e realizar a verificação dentro de sua sessão. A/B/C ainda não foram executados.**

Identidades consultadas nesta preparação:

| Item | Valor observado |
|---|---|
| Usuário efetivo do executor | `DESKTOP-9AB21K5\Micro` |
| SID efetivo/pessoal | `S-1-5-21-1211977137-300896993-751103003-1001` |
| USERPROFILE efetivo | `C:\Users\Micro` |
| Conta dedicada | `PE_Tester`, existente e habilitada |
| SID da conta dedicada | `S-1-5-21-1211977137-300896993-751103003-1006` |
| Pasta `C:\Users\PE_Tester` | Não encontrada nesta consulta; o caminho efetivo deverá ser confirmado ao entrar na conta |
| HKCU, pastas persistentes e acesso efetivo como PE_Tester | Ainda não verificados nessa identidade |

Não se pediu nem armazenou senha. Não se criou conta, alterou grupo administrativo, instalou VM ou executou o jogo sob Micro.

### Pacote local privado pronto

Foi criado `C:\PE_ModTest_20261023`, fora dos repositórios. Antes de copiar binários, foi aplicada e conferida ACL sem herança, permitindo acesso apenas a Micro, PE_Tester, SYSTEM e administradores. PE_Tester recebeu Modify somente nessa pasta; nenhum privilégio administrativo foi concedido.

Uma tentativa anterior de preparar o pacote em Documentos Públicos foi rejeitada pela revisão automática por exposição dos binários a outras contas locais. Essa tentativa não foi executada. O destino privado foi aceito e preparado em seguida.

| Condição preparada | Conteúdo | Arquivos originais conferidos |
|---|---|---|
| `A_Original` | Cópia da WorkingCopy, sem loader | 47/47 correspondem à baseline |
| `B_Loader` | Cópia da LoaderCopy; DLL VisualProbe renomeada para `.dll.disabled` somente nessa cópia | 47/47 correspondem à baseline |
| `C_Visual` | Cópia da LoaderCopy; VisualProbe e marcador de substituição ativos | 47/47 correspondem à baseline |

WorkingCopy, LoaderCopy anteriores e suas evidências não foram modificadas. As configurações de loader já existentes foram reutilizadas. Os logs antigos presentes nas novas cópias não constituem evidência de uma nova execução.

Evidências da preparação: [manifesto](../../ProblematicSubjects_Research/ProfileValidation/package_manifest.json), [integridade](../../ProblematicSubjects_Research/ProfileValidation/preparation_integrity.json), [ACL privada](../../ProblematicSubjects_Research/ProfileValidation/private_package_acl.txt) e [script original do roteiro](../../ProblematicSubjects_Research/ProfileValidation/Run-PE-Test.ps1).

### Procedimento manual curto

1. Pelo menu de usuário do Windows, entre em **PE_Tester**. Faça a autenticação localmente; não envie senha ao chat. Não é necessário encerrar sua sessão pessoal.
2. Nessa conta, abra PowerShell **sem “Executar como administrador”**. Execute uma condição por vez:

   ```powershell
   powershell -NoProfile -File C:\PE_ModTest_20261023\Run-PE-Test.ps1 -Condition A
   powershell -NoProfile -File C:\PE_ModTest_20261023\Run-PE-Test.ps1 -Condition B
   powershell -NoProfile -File C:\PE_ModTest_20261023\Run-PE-Test.ps1 -Condition C
   ```

3. Siga os prompts de cada execução: menu estável → clique real em **CREDITS** → controle real de voltar → menu. Não clique START ou SAVEFILE. Em C, observe o fundo antes/depois de voltar; o roteiro depois desativa o marcador e solicita observar a restauração.
4. Para cada captura, confirme no terminal, volte à janela do jogo em até cinco segundos e deixe-a inteiramente visível, preferencialmente no segundo monitor. Mantenha resolução 1920×1080, mesmo foco, monitor e estado nas três condições. Se o jogo alterar a resolução ou o monitor solicitado, registre isso; não trate capturas diferentes como equivalentes. Aguarde pelo menos dez segundos no menu inicial e cinco nas demais telas antes de confirmar os prompts.
5. Descreva os cliques que realmente fez e a presença/ausência de SETTINGS e SAVEFILE no prompt de observações. Encerre pelo QUIT. Preserve as pastas geradas em `Documents\PE_TestEvidence` dentro do perfil PE_Tester e disponibilize-as para revisão.

Se o Windows bloquear scripts ou houver erro de acesso/identidade, pare e informe a mensagem. Não altere políticas globais nem tente o perfil pessoal como alternativa. O script não coleta credenciais. O roteiro C foi preparado para uma passagem; depois da restauração, outra passagem exige reativação explícita do marcador na cópia C, sem tocar no original.

### Verificações e limites do roteiro

Antes de abrir o jogo, o script exige nome/SID de PE_Tester, USERPROFILE compatível com a pasta especial de perfil, HKCU Volatile Environment, HKEY_USERS do SID dedicado e ProfileList do Windows. Também bloqueia execução elevada, confere leitura e hashes dos 47 arquivos e registra os locais persistentes conhecidos e preferências antes/depois. O funcionamento desses gates na sessão PE_Tester ainda não foi executado; a sintaxe do script foi validada por parser.

As pastas inventariadas ficam sob AppData LocalLow/Local/Roaming, `Babus Games\Problematic Subjects`; HKCU `Software\Babus Games\Problematic Subjects` é registrado por nomes e hashes dos valores. Isso cobre locais conhecidos, sem alegar descoberta de todo armazenamento possível. Os resultados e logs ficam no perfil dedicado. O acesso de PE_Tester aos executáveis ainda deve ser confirmado pelo próprio script; a ACL preparada, isoladamente, não prova a execução.

As capturas usam a ferramenta de screenshot já disponível, copiando a imagem visível da janela após estabilização. Não usam a chamada de screenshot em Update do plugin como evidência principal. Isso registra a imagem composta exibida pelo Windows; não é um novo hook comprovado de fim de frame do Unity. Janelas sobrepostas, resolução diferente ou animação em transição invalidam a comparação. O roteiro não clica automaticamente e não invoca callbacks internos. Uma captura de Créditos só comprova o resultado visual em conjunto com as observações dos cliques realmente executados.

**Resultado atualizado A/B/C:** preparado, pendente de execução na sessão PE_Tester. **Navegação, rótulos, diferenças entre condições, novas exceções e persistência após transição:** ainda não verificados. A causa dos rótulos permanece desconhecida. A recomendação atual é executar este roteiro sob a conta dedicada e revisar suas evidências antes de qualquer experiência com personagens. Não há inviabilidade de modding demonstrada.

## Registro histórico da verificação anterior

## Resultado desta etapa

**Impedido antes da execução: não foi estabelecido um ambiente com isolamento real e verificável do perfil. Nenhuma condição A/B/C foi executada nesta etapa.**

O usuário autorizou exclusivamente navegação e renderização e determinou expressamente: “Se não houver um método seguro e verificável de isolamento, documente o impedimento e não realize testes que possam alterar saves pessoais.” Esse limite foi aplicado antes de iniciar qualquer cópia do jogo.

O primeiro experimento continua demonstrando troca e restauração de um fundo em memória. Ele não comprova navegação ou estabilidade completa. A nova verificação não confirmou nem refutou a estabilidade do loader/plugin.

## Verificação do isolamento

Foram feitas somente consultas locais de leitura e gravação de documentação/evidências novas. Não foram criadas contas, instalados hipervisores, habilitados recursos do Windows, solicitada troca de sessão ou alteradas preferências.

| Verificação | Observação | Consequência |
|---|---|---|
| Windows Sandbox | `C:\Windows\System32\WindowsSandbox.exe` e o cliente consultado não encontrados | Sandbox não disponível para execução imediata pelos caminhos consultados |
| Estado dos recursos Sandbox e Hyper-V | `Get-WindowsOptionalFeature` retornou “A operação solicitada requer elevação” | Estado de instalação/habilitação não confirmado; ausência do executável não foi tratada como prova definitiva sobre todos os recursos |
| Hipervisor | `HyperVisorPresent = false` | Nenhum hipervisor ativo identificado pela consulta |
| Comando Hyper-V | `Get-VM` indisponível | Não foi possível inventariar VMs por esse módulo |
| Outras ferramentas | Executáveis ausentes nos caminhos convencionais consultados de VirtualBox e VMware | Nenhuma VM pronta identificada; não é uma busca exaustiva de todo o disco |
| Contas locais | Conta pessoal habilitada e contas `CodexSandboxOffline`/`CodexSandboxOnline`; contas administrativas/convidado listadas estavam desabilitadas | As contas Codex não foram tratadas como contas interativas de teste do jogo nem reutilizadas para esse fim |
| Dados atuais do jogo | 22 entradas na chave de preferências consultada; pasta persistente esperada sem arquivos | Executar outra cópia no mesmo perfil continuaria expondo essas preferências a gravações |

Uma aprovação de comando fora do sandbox do executor não concede, por si só, elevação administrativa do Windows. O erro do DISM foi uma limitação do token administrativo, não uma rejeição automática de aprovação.

Não se adotou mudança de variáveis de ambiente ou mera cópia do executável como prova de isolamento de HKCU/PlayerPrefs. Também não se renomeou produto/empresa ou alterou configuração serializada do jogo para tentar desviar os dados persistentes.

Estratégia necessária para retomar: disponibilizar uma conta Windows interativa dedicada ou uma VM/Sandbox com perfil descartável, sessão gráfica e renderização compatível. Antes de executar o jogo, verificar a identidade efetiva da sessão, seu HKCU e as pastas persistentes; manter saves e preferências da conta pessoal fora desse ambiente. Disponibilidade, credenciais e controles gráficos desse ambiente ainda não foram estabelecidos nesta sessão.

Evidência: [isolation_preflight.json](../../ProblematicSubjects_Research/Evidence/Stability_20261023/isolation_preflight.json).

## Matriz A/B/C

As células “não executado” não significam falha funcional do jogo. Significam que o pré-requisito obrigatório de isolamento não foi atendido.

| Verificação | A — original limpo | B — BepInEx, plugin desativado | C — BepInEx com substituição |
|---|---|---|---|
| Iniciar a condição no perfil isolado | Não executado | Não executado | Não executado |
| Menu inicial, frame completo | Não capturado | Não capturado | Não capturado |
| SETTINGS e SAVEFILE | Não verificado | Não verificado | Não verificado |
| Cliques reais e resposta dos botões | Não executado | Não executado | Não executado |
| Entrada em Créditos | Não executado | Não executado | Não executado |
| Retorno ao menu | Não executado | Não executado | Não executado |
| Diferenças de UI e transições | Sem comparação nova | Sem comparação nova | Sem comparação nova |
| Fundo substituído | Não aplicável | Não aplicável | Não testado nesta etapa |
| Restauração do sprite | Não aplicável | Não aplicável | Não testada nesta etapa |
| Logs/exceções de execução | Não gerados | Não gerados | Não gerados |

O roteiro permanece `Menu → Créditos → Voltar`, sem iniciar progressão nem abrir operações de save. Não foram invocados callbacks internos para substituir interação real.

## Rótulos ausentes e captura

**Causa: desconhecida.** Não há novas capturas equivalentes das três condições que permitam atribuir o desaparecimento de SETTINGS/SAVEFILE ao procedimento de captura, segundo plano, BepInEx, plugin ou outro comportamento.

No experimento anterior, ambos os rótulos estavam presentes no quadro inicial e ausentes nos quadros de troca e restauração. A captura alternativa em textura era solicitada pelo diagnóstico em `Update`, sem sincronização comprovada com o fim da renderização. Isso é uma limitação metodológica e uma hipótese a testar, não uma causa demonstrada. A ausência após restaurar o fundo também impede atribuir o problema simplesmente à nova imagem.

Nesta etapa não foi implementada nem validada captura ao fim do frame. Não se apresentou uma recaptura dos arquivos antigos como evidência nova. A falha anterior de automação (`foreground window did not report a process id`) continua sendo uma limitação histórica; o controle da janela em uma nova sessão isolada ainda precisa ser verificado.

## Ciclo de vida do recurso

| Evento solicitado | Evidência disponível |
|---|---|
| Criação do SpriteRenderer | Instante de criação não medido. O log anterior registra apenas a descoberta de `MainMenu/Background` |
| Substituição | Executada anteriormente na instância `16938`, com log APPLIED; sem nova execução |
| Persistência ao navegar | Desconhecida: nenhuma navegação foi realizada |
| Retorno ao menu | Não observado |
| Necessidade de reaplicar | Desconhecida; não foi construído sistema dinâmico |
| Referência destruída/recriada | Não observado em transição; não se pode concluir que a mesma instância sobreviva |
| Restauração | Confirmada anteriormente no mesmo renderer, sem comprovação após navegar |

## Evidências e preservação

Novas evidências desta etapa:

- [isolation_preflight.json](../../ProblematicSubjects_Research/Evidence/Stability_20261023/isolation_preflight.json): verificações e limitações do ambiente.
- [preservation_check.json](../../ProblematicSubjects_Research/Evidence/Stability_20261023/preservation_check.json): integridade das cópias e comparação dos dados consultados.
- [userdata_stability_before.json](../../ProblematicSubjects_Research/Evidence/userdata_stability_before.json) e [userdata_stability_after.json](../../ProblematicSubjects_Research/Evidence/userdata_stability_after.json): snapshots de arquivos e hashes dos valores do registro, sem reproduzir seus valores.

Os 47 arquivos preexistentes conferidos em cada uma das duas cópias, WorkingCopy e LoaderCopy do experimento anterior, correspondem aos hashes da baseline anterior. Os snapshots dos dados persistentes consultados antes/depois desta verificação são iguais: zero arquivos na pasta esperada e 22 entradas de registro. Isso verifica esses locais; não é uma auditoria de todos os possíveis mecanismos de armazenamento do jogo.

ZIP e pasta Original não foram usados nem modificados. As evidências anteriores foram preservadas. Não foram copiadas imagens/cenas/código proprietários para o projeto autoral. Gym Discipline não foi alterado. Não houve operações de save, alterações de mecânicas, substituição de personagens, commit ou push.

As capturas e os logs anteriores permanecem referenciados em [MODDING_FEASIBILITY_REPORT.md](MODDING_FEASIBILITY_REPORT.md), com seu escopo original. Não foram produzidos screenshots de menu/tela secundária/retorno nesta etapa, porque o jogo não foi executado.

## Critérios de aprovação

Os critérios de funcionamento original, navegação com loader, troca durante navegação, controles, restauração após transição e ausência de diferenças inexplicadas **não foram atendidos como verificação nesta etapa**. Permanecem pendentes, não reprovados por falha funcional demonstrada.

A conferência dos arquivos passou para o conjunto verificado. A preservação do perfil pessoal nesta etapa foi mantida ao não executar o jogo; a segurança de uma nova execução depende do ambiente isolado ainda indisponível.

**O experimento de estabilidade não pode ser declarado tecnicamente satisfatório.**

## Conclusão e recomendação

**B — corrigir um problema técnico específico antes de continuar: estabelecer e verificar um perfil interativo realmente isolado.** Depois disso, executar a matriz autorizada com interação real e capturas equivalentes ao fim do frame, para investigar os rótulos sem modificar o jogo original.

Não há evidência suficiente para prosseguir para um personagem. Também não há inviabilidade de modding demonstrada que justifique a alternativa C da solicitação atual.

A preparação de outro ambiente e a próxima execução não foram iniciadas automaticamente. O impedimento aqui é de isolamento disponível nesta sessão, não uma conclusão de impossibilidade de criar esse ambiente em geral.
