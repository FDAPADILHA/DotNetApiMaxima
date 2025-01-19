import { Component } from '@angular/core';
import { UsuarioService } from '../../usuario.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData = { login: '', senha: '' };  // Mantido login e senha para a compatibilidade com o backend
  mensagemErro = '';
  mensagemSucesso = '';  // Variável para mensagem de sucesso

  constructor(private usuarioService: UsuarioService, private router: Router) {}

  onSubmit() {
    this.usuarioService.login(this.loginData).subscribe(
      (response) => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          const usuario = response.usuario;
          console.log('Usuário logado:', usuario);
          this.mensagemSucesso = 'Login realizado com sucesso!';
          this.router.navigate(['/departamento']);
        } else {
          this.mensagemErro = 'Erro ao fazer login. Tente novamente.';
        }
      },
      (erro) => {
        console.error('Erro ao fazer login', erro);
        this.mensagemErro = `Credenciais inválidas. Detalhes: ${erro.message || erro}`;
      }
    );
  }

}
