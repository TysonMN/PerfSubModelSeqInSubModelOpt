namespace SubModelSeqInSubModelOpt.Core

module UsersSubModel =
    open Bogus
    open Elmish.WPF

    let faker = Faker "en"

    type User = {
        Id: int
        FullName: string
        Company: string
        PhoneNumber : string
    }

    let genUser (id: int) = {
        Id = id
        FullName = faker.Name.FullName()
        Company = faker.Company.CompanyName()
        PhoneNumber = faker.Phone.PhoneNumber()
    }

    type Model = {
        Users: User list
    }

    let init() ={Users = []}

    type Msg =
        | Reset
        | LoadData

    let update msg m =
        match msg with
            | Reset -> init()
            | LoadData -> {m with Users = [for i = 1 to 200 do genUser i]}

    let bindings () : Binding<Model, Msg> list = [
        "Users" |> Binding.subModelSeq(
            (fun m -> m.Users),
            (fun e -> e.Id),
            (fun () -> [
                "Id" |> Binding.oneWay (fun (_, e) -> e.Id)
                "FullName" |> Binding.oneWay (fun (_, e) -> e.FullName)
                "Company" |> Binding.oneWay (fun (_, e) -> e.Company)
                "PhoneNumber" |> Binding.oneWay (fun (_, e) -> e.PhoneNumber)
            ])
        )

        "Reset" |> Binding.cmd Reset
        "LoadData" |> Binding.cmd LoadData
    ]

module ContainerModel =
    open Elmish.WPF
    open UsersSubModel

    type SubModels =
        | Users of UsersSubModel.Model

    type Model = {
        SubModel: SubModels option
    }

    let init () = {SubModel = None}

    type Msg =
        | Reset
        | ShowUsers
        | UsersMsg of UsersSubModel.Msg

    let update msg m =
        match msg with
            | Reset -> init()
            | ShowUsers -> { m with SubModel = Some <| Users (UsersSubModel.init ())}
            | UsersMsg msg' ->
                match m.SubModel with
                | Some (Users m') -> { m with SubModel = UsersSubModel.update msg' m' |> Users |> Some }
                | _ -> m

    let bindings () : Binding<Model, Msg> list = [
        "Reset" |> Binding.cmd Reset
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
    open Elmish
    open Elmish.WPF

    let usersDesignVm = ViewModel.designInstance (UsersSubModel.init ()) (UsersSubModel.bindings ())
    let containerDesignVm = ViewModel.designInstance (ContainerModel.init ()) (ContainerModel.bindings ())

    let main window =
        WpfProgram.mkSimple ContainerModel.init ContainerModel.update ContainerModel.bindings
        |> WpfProgram.startElmishLoop window