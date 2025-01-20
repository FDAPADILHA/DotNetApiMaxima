import { Component } from '@angular/core';
import { UsuarioService } from '../../usuario.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData = { login: '', senha: '' };
  isLoading = false;

  constructor(
    private usuarioService: UsuarioService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  onSubmit(): void {
    if (this.isLoading) return;
    this.isLoading = true;

    this.usuarioService.login(this.loginData).subscribe(
      (response) => this.handleLoginResponse(response),
      (erro) => this.handleLoginError(erro)
    );
  }

  private handleLoginResponse(response: any): void {
    this.isLoading = false;
    if (response?.token) {
      localStorage.setItem('token', response.token);
      console.log('Usuário logado:', response.usuario);

      this.snackBar.open('Login realizado com sucesso!', 'Fechar', {
        duration: 3000,
        panelClass: ['success-snackbar']
      });

      this.router.navigate(['/departamento']);
    } else {
      this.snackBar.open('Erro ao fazer login. Tente novamente.', 'Fechar', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
    }
  }

  private handleLoginError(erro: any): void {
    this.isLoading = false;
    console.error('Erro ao fazer login', erro);

    this.snackBar.open(`Credenciais inválidas. Detalhes: ${erro.message || erro}`, 'Fechar', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }
}
