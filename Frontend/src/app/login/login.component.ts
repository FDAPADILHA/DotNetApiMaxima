import { Component } from '@angular/core';
import { UsuarioService } from './usuario.service'; // Importe o serviço corretamente
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData = { username: '', password: '' };
  mensagemErro = '';

  constructor(private usuarioService: UsuarioService, private router: Router) {}

  onSubmit() {
    this.usuarioService.login(this.loginData).subscribe(
      (response) => {
        // Armazenar o token (por exemplo, em localStorage ou outro mecanismo)
        localStorage.setItem('token', response.token);

        // Agora, consulte os usuários cadastrados
        const userId = response.usuario.id; // ID do usuário logado
        this.usuarioService.consultarUsuarios([userId]).subscribe(
          (usuarios) => {
            console.log('Usuários cadastrados:', usuarios);
            // Redirecionar para a tela de departamentos ou qualquer outra rota
            this.router.navigate(['/departamento']);
          },
          (erro) => {
            console.error('Erro ao consultar usuários', erro);
            this.mensagemErro = 'Não foi possível consultar os usuários cadastrados.';
          }
        );
      },
      (erro) => {
        console.error('Erro ao fazer login', erro);
        this.mensagemErro = 'Credenciais inválidas. Tente novamente.';
      }
    );
  }
}
