import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  isVisible: boolean = true;
  isCadastroVisible: boolean = false; 

  constructor(
    private router: Router
  ) {}

  toggleNavbar() {
    this.isVisible = !this.isVisible;
  }

  toggleCadastroTools() {
    this.isCadastroVisible = !this.isCadastroVisible;
  }

  goToDepartamento() {
    this.router.navigate(['/departamento']);
  }

  goToProduto() {
    this.router.navigate(['/produto']);
  }

  logout() {
    console.log("Deslogando do sistema...");
  }
}
