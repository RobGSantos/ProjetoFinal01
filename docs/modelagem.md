# Modelagem inicial da API

## Visão geral

A API possui uma modelagem voltada para gerenciamento de cursos, estudantes e matrículas. A autenticação e autorização serão feitas com ASP.NET Core Identity, usando uma entidade própria chamada `ApplicationUser`.

A entidade `Student` não substitui o usuário do Identity. Ela representa o perfil de estudante e possui uma chave estrangeira para o usuário autenticável.

Decisão de modelagem adotada:

```text
Student será uma entidade própria da aplicação e referenciará o usuário do Identity por meio de ApplicationUserId.
```

Essa decisão mantém separados os dados de autenticação, gerenciados pelo Identity, e os dados de negócio do estudante, gerenciados pela aplicação.

## Entidades principais

### ApplicationUser

Representa o usuário do sistema usado pelo ASP.NET Core Identity.

Tabela base esperada: `AspNetUsers`

Principais responsabilidades:

| Campo        | Tipo   | Observação                             |
| ------------ | ------ | -------------------------------------- |
| Id           | string | Chave primária do Identity             |
| UserName     | string | Nome de usuário                        |
| Email        | string | E-mail de autenticação                 |
| PasswordHash | string | Hash da senha gerenciado pelo Identity |

Regras:

* Usado para login, senha, roles e geração de JWT.
* Pode estar relacionado a um perfil de estudante.
* Nem todo usuário precisa ser estudante, pois também existirão usuários Admin e Instructor.

---

### Student

Representa o perfil de estudante da aplicação.

Tabela lógica: `Students`

| Coluna            | Tipo     | Obrigatório | Observação                                      |
| ----------------- | -------- | ----------: | ----------------------------------------------- |
| Id                | int      |         Sim | Chave primária                                  |
| NomeCompleto      | string   |         Sim | Nome completo do estudante                      |
| Email             | string   |         Sim | E-mail cadastral do estudante                   |
| DataCadastro      | DateTime |         Sim | Data de cadastro do perfil                      |
| ApplicationUserId | string   |         Sim | FK para `ApplicationUser`                       |
| IsDeleted         | bool     |         Sim | Indica exclusão lógica do perfil de estudante   |

Regras:

* `Email` deve ser único.
* `ApplicationUserId` deve ser único.
* Um usuário do Identity pode ter no máximo um perfil de estudante.
* Um estudante pode possuir várias matrículas.
* A remoção de estudante será tratada como soft delete, marcando `IsDeleted = true`.

Relacionamentos:

```text
ApplicationUser 1 ─── 0..1 Student
Student 1 ─── N Enrollment
```

---

### Course

Representa um curso disponível na API.

Tabela lógica: `Courses`

| Coluna       | Tipo     | Obrigatório | Observação                              |
| ------------ | -------- | ----------: | --------------------------------------- |
| Id           | int      |         Sim | Chave primária                          |
| Titulo       | string   |         Sim | Título do curso                         |
| Descricao    | string   |         Sim | Descrição do curso                      |
| Categoria    | string   |         Sim | Categoria do curso                      |
| CargaHoraria | int      |         Sim | Carga horária em horas                  |
| DataCriacao  | DateTime |         Sim | Data de criação do curso                |
| IsDeleted    | bool     |         Sim | Indica exclusão lógica do curso         |

Regras:

* O título deve ter no mínimo 3 caracteres.
* Um curso pode possuir várias matrículas.
* A validação de tamanho mínimo do título será tratada na entrada da API por DTO/validação.
* A remoção de curso será tratada como soft delete, marcando `IsDeleted = true`.
* Consultas públicas de cursos devem retornar, por padrão, apenas cursos com `IsDeleted = false`.

Relacionamento:

```text
Course 1 ─── N Enrollment
```

---

### Enrollment

Representa a matrícula de um estudante em um curso.

Tabela lógica: `Enrollments`

| Coluna        | Tipo     | Obrigatório | Observação                                   |
| ------------- | -------- | ----------: | -------------------------------------------- |
| Id            | int      |         Sim | Chave primária                               |
| StudentId     | int      |         Sim | FK para `Student`                            |
| CourseId      | int      |         Sim | FK para `Course`                             |
| Status        | string   |         Sim | Status da matrícula, como ativo ou cancelado |
| DataMatricula | DateTime |         Sim | Data da matrícula                            |

Regras:

* Um estudante não pode se matricular duas vezes no mesmo curso.
* A combinação `StudentId + CourseId` deve ser única.
* A matrícula pertence obrigatoriamente a um estudante e a um curso.
* O status controla a situação da matrícula, por exemplo: ativo ou cancelado.

Relacionamentos:

```text
Enrollment N ─── 1 Student
Enrollment N ─── 1 Course
```

---

## Índices e restrições

| Entidade   | Índice/Restrição           | Tipo       | Finalidade                                                |
| ---------- | -------------------------- | ---------- | --------------------------------------------------------- |
| Student    | Email                      | Único      | Impedir dois estudantes com o mesmo e-mail                |
| Student    | ApplicationUserId          | Único      | Impedir dois perfis Student para o mesmo usuário Identity |
| Course     | Categoria                  | Não único  | Melhorar consultas e filtros por categoria de curso       |
| Enrollment | StudentId + CourseId       | Único      | Impedir matrícula duplicada no mesmo curso                |

## Regras de exclusão

Os relacionamentos principais devem evitar exclusão em cascata automática.

Recomendação inicial:

```text
DeleteBehavior.Restrict
```

Motivo:

* Evita apagar matrículas automaticamente ao remover estudante.
* Evita apagar matrículas automaticamente ao remover curso.
* Preserva maior controle sobre regras de negócio.

Além disso, `Student` e `Course` possuem a coluna `IsDeleted` para permitir exclusão lógica.

Regras de soft delete:

* Remover estudante deve marcar `Student.IsDeleted = true`.
* Remover curso deve marcar `Course.IsDeleted = true`.
* Consultas normais devem ignorar registros marcados como `IsDeleted = true`.
* Matrículas não usam soft delete nesta modelagem inicial; a situação da matrícula será controlada por `Status`.

## Diagrama simples

```text
ApplicationUser
      │
      │ 1 para 0..1
      ▼
   Student
      │
      │ 1 para N
      ▼
 Enrollment
      ▲
      │ N para 1
    Course
```

## Observações

* As tabelas do Identity serão gerenciadas pelo ASP.NET Core Identity.
* As entidades de negócio da aplicação são `Student`, `Course` e `Enrollment`.
* Validações de entrada, como e-mail válido e título com tamanho mínimo, serão tratadas por DTOs e validações da API.
* Restrições de unicidade devem ser garantidas no banco por índices únicos.
* O índice em `Course.Categoria` foi planejado para consultas frequentes por categoria.
* A exclusão lógica foi prevista em `Student` e `Course` por meio da coluna `IsDeleted`.
