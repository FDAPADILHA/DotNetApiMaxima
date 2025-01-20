import { Component } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  isVisible: boolean = true;
  isCadastroVisible: boolean = false; 
  toggleNavbar() {
    this.isVisible = !this.isVisible;
  }

  toggleCadastroTools() {
    this.isCadastroVisible = !this.isCadastroVisible;
  }

  logout() {
    console.log("Deslogando do sistema...");
  }
}
