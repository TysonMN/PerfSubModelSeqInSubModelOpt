namespace SubModelSeqInSubModelOpt.Core

module UsersSubModel =
    open Elmish.WPF

    type User =
      { Id: int }

    let genUser (id: int) =
      { Id = id }

    type Model =
      { Users: User list
        SelectedUser: int option }

    let init () =
      { Users = [for i = 1 to 5 do genUser i]
        SelectedUser = None }

    type Msg =
        | Select of int option

    let update msg m =
        match msg with
            | Select userId -> { m with SelectedUser = userId }

    let bindings () : Binding<Model, Msg> list = [
        "Users" |> Binding.subModelSeq(
            (fun m -> m.Users),
            (fun e -> e.Id),
            (fun () -> [ "Id" |> Binding.oneWay (fun (_, e) -> e.Id) ]))

        "SelectedUser" |> Binding.subModelSelectedItem("Users", (fun m -> m.SelectedUser), Select)
    ]

module ContainerModel =
    open Elmish.WPF

    type SubModels =
        | Users of UsersSubModel.Model

    type Model =
      { SubModel: SubModels option }

    let init () = { SubModel = None }

    type Msg =
        | ShowUsers
        | UsersMsg of UsersSubModel.Msg

    let update msg m =
        match msg with
            | ShowUsers -> { m with SubModel = Some <| Users (UsersSubModel.init ())}
            | UsersMsg msg' ->
                match m.SubModel with
                | Some (Users m') -> { m with SubModel = UsersSubModel.update msg' m' |> Users |> Some }
                | _ -> m

    let bindings () : Binding<Model, Msg> list = [
        "ShowUsers" |> Binding.cmd ShowUsers

        "UsersVisible" |> Binding.oneWay
            (fun m -> match m.SubModel with Some (Users _) -> true | _ -> false)

        "UserModel" |> Binding.subModelOpt(
            (fun m -> match m.SubModel with Some (Users m') -> Some m' | _ -> None),
            snd,
            UsersMsg,
            UsersSubModel.bindings)
    ]

module MainApp =
    open Serilog
    open Serilog.Extensions.Logging
    open Elmish.WPF

    let main window =
        let logger =
            LoggerConfiguration()
              .MinimumLevel.Override("Elmish.WPF.Update", Events.LogEventLevel.Verbose)
              .MinimumLevel.Override("Elmish.WPF.Bindings", Events.LogEventLevel.Verbose)
              .MinimumLevel.Override("Elmish.WPF.Performance", Events.LogEventLevel.Verbose)
              .WriteTo.Seq("http://localhost:5341")
              .CreateLogger()

        WpfProgram.mkSimple ContainerModel.init ContainerModel.update ContainerModel.bindings
        |> WpfProgram.withLogger (new SerilogLoggerFactory(logger))
        |> WpfProgram.startElmishLoop window